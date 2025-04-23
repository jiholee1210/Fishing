using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class QuestNpcManager : MonoBehaviour
{
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] GameObject reqFishPrefab;

    [SerializeField] private Transform normalQuestParent;
    [SerializeField] private Transform epicQuestParent;
    [SerializeField] Transform reqParent;

    [SerializeField] Transform questDetail;
    [SerializeField] private TMP_Text timer;

    private List<QuestData> npcQuest;
    private List<int> completeQuest;
    private List<NormalQuest> normalQuests;

    private List<PlayerFish> playerFish;
    private PlayerData playerData;

    private int npcID;

    private int nextQuestReset = 300;
    private float saveInterval = 60f;
    private int curTime = 0;
    private float saveTime = 0f;
    
    [SerializeField] private PlayerInventory playerInventory;

    WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

    void Start()
    {
        completeQuest = DataManager.Instance.playerData.completeQuest;
        playerFish = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        npcQuest = DataManager.Instance.npcQuestList;
        normalQuests = DataManager.Instance.npcQuest.normalQuests;
        StartCoroutine(QuestTimer());
    }

    void Update()
    {
        saveTime += Time.deltaTime;

        if(saveTime >= saveInterval) {
            saveTime = 0f;
            DataManager.Instance.npcQuest.timer = curTime;
            DataManager.Instance.SaveQuestNpcData();
        }
    }

    private IEnumerator QuestTimer() {
        yield return new WaitForSeconds(0);

        // curtime 을 1씩 증가시켜서 nextQuestReset 보다 커지거나 같게 되었을 때 퀘스트 생성
        // 1초마다 반복하는 식으로 진행
        curTime = DataManager.Instance.npcQuest.timer;
        int remainTime = nextQuestReset - curTime;
        int min;
        int sec;
        while(true) {
            if(remainTime == 0) {
                normalQuests.Clear();
                CreateQuest(1);
                CreateQuest(3);
                CreateQuest(4);
                CreateQuestItems();
                questDetail.gameObject.SetActive(false);
                curTime = -1;
                Debug.Log("노말 퀘스트 초기화");
                DataManager.Instance.SaveQuestNpcData();
            }

            curTime += 1;
            remainTime = nextQuestReset - curTime;
            
            min = remainTime / 60;
            sec = remainTime % 60;
            timer.text = min.ToString("D2") + ":" + sec.ToString("D2");
            yield return waitOneSecond;
        }
    }

    private void CreateQuest(int groundType) {
        for(int i = 0; i < 5; i++) {
            int fishCount = Random.Range(1, 4);
            int price = 0;
            NormalQuest normalQuest = new();
            QuestRequirement[] reqFish = new QuestRequirement[fishCount];
            List<int> fishID = new(); 

            foreach(var fish in DataManager.Instance.fishDataDict) {
                if(groundType == 1) {
                    if(fish.Value.habitat == (Habitat)groundType || fish.Value.habitat == (Habitat)(groundType+1)) {
                        fishID.Add(fish.Key);
                    }
                }
                else if(fish.Value.habitat == (Habitat)groundType) {
                    fishID.Add(fish.Key);
                }
            }

            for(int j = 0; j < fishCount; j++) {
                int temp = j;
                int randomID = fishID[Random.Range(0, fishID.Count)];
                int grade = Random.Range(0, 4);

                reqFish[temp] = new();
                reqFish[temp].fishID = randomID;
                reqFish[temp].grade = grade;

                price += (int)(DataManager.Instance.GetFishData(randomID).price * 2.5f * (1 + grade * 0.5f));
            }
            normalQuest.receive = groundType;
            normalQuest.complete = groundType;
            normalQuest.questRequirements = reqFish;
            normalQuest.rewardGold = price;
            normalQuests.Add(normalQuest);
            Debug.Log("노말 퀘스트 추가");
        }
    }

    private void SetDetail(QuestData questData, int index) {
        SoundManager.Instance.ButtonClick();
        questDetail.gameObject.SetActive(true);
        questDetail.GetChild(5).gameObject.SetActive(false);
        
        TMP_Text name = questDetail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text desc = questDetail.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text gold = questDetail.GetChild(3).GetComponent<TMP_Text>();
        TMP_Text reward = questDetail.GetChild(7).GetComponent<TMP_Text>();
        
        Button btn = questDetail.GetChild(5).GetComponent<Button>();
        questDetail.GetChild(5).gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => CompleteQuest(questData, index));
      
        name.text = questData.questName;
        desc.text = questData.desc;
        gold.text = questData.rewardGold + " " + LocalizationSettings.StringDatabase.GetLocalizedString("DialogTable", "coin");
        reward.text = questData.reward;

        foreach(Transform req in reqParent) {
            Destroy(req.gameObject);
        }

        int len = questData.requirements.Length;

        for(int i = 0; i < len; i++) {
            GameObject questReq = Instantiate(reqFishPrefab, reqParent);
            RectTransform rect = questReq.GetComponent<RectTransform>();

            questReq.transform.GetChild(1).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[questData.requirements[i].grade];
            questReq.transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.GetFishData(questData.requirements[i].fishID).fishIcon;

            float xPos = (-60 * (len - 1)) + (120 * i);
            rect.anchoredPosition = new Vector2(xPos, 0);
        }
    }

    public void CompleteQuest(QuestData questData, int index) {
        List<PlayerFish> available = new(playerFish);
        List<int> list = new();

        foreach(var requirement in questData.requirements) {
            bool fishFound = false;
            for(int i = 0; i < available.Count; i++) {
                if(available[i].fishID == requirement.fishID && available[i].grade == requirement.grade) {
                    available.RemoveAt(i);
                    list.Add(i);
                    fishFound = true;
                    break;
                }   
            }
            if(!fishFound) {
                Debug.Log("필요한 물고기가 부족합니다.");
                EventManager.Instance.ReqFish();
                SoundManager.Instance.QuestFail();
                return;
            }
        }
        SoundManager.Instance.QuestClear();
        foreach(int i in list) {
            playerFish[i] = null;
        }
        EventManager.Instance.ClearQuest(questData.questID);

        playerData.gold += questData.rewardGold;

        if(questData.questID < 1000) {
            completeQuest.Add(questData.questID);
        }
        questDetail.gameObject.SetActive(false);
        if(questData.isEpic) {
            npcQuest.Remove(questData);
            foreach(var id in questData.nextQuest) {
                npcQuest.Add(DataManager.Instance.GetQuestData(id));
            }
        }
        else {
            normalQuests.RemoveAt(index);
        }
        CreateQuestItems();
        DataManager.Instance.SavePlayerData();
        DataManager.Instance.SaveInventoryData();
        DataManager.Instance.SaveQuestNpcData();
    }
    

    private void CreateQuestItems()
    {
        // 기존 퀘스트 아이템들 제거
        foreach(Transform child in normalQuestParent)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in epicQuestParent)
        {
            Destroy(child.gameObject);
        }
        // 새로운 퀘스트 아이템 생성
        Debug.Log("퀘스트 개수 : " + npcQuest.Count);
        for(int i = 0; i < npcQuest.Count; i++)
        {
            GameObject quest;
            int index = i;
            // npc id와 퀘스트 완료 npc 아이디가 같을때만 출력하도록
            if(npcQuest[index].receive != npcID) continue;

            quest = Instantiate(questItemPrefab, epicQuestParent);
            
            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -(index * (rectTransform.rect.height + 10f)));
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = npcQuest[index].questName; 
            
            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(npcQuest[index], index));
        }

        int count = 0;
        for(int i = 0; i < normalQuests.Count; i++) {
            GameObject quest;
            int index = i;
            QuestData questData = ScriptableObject.CreateInstance<QuestData>();
            if(normalQuests[index].receive != npcID) continue;

            questData.questID = 1001;
            questData.isEpic = false;
            questData.receive = normalQuests[index].receive;
            questData.complete = normalQuests[index].complete;            
            questData.requirements = normalQuests[index].questRequirements;
            questData.rewardGold = normalQuests[index].rewardGold;

            quest = Instantiate(questItemPrefab, normalQuestParent);

            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -(count++ * (rectTransform.rect.height + 10f)));
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = questData.questName; 

            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(questData, index));
        }
    }

    public void SetQuest(GameObject _npcObject) {
        npcID = _npcObject.GetComponent<IQuest>().GetNpcID();
        CreateQuestItems();
    }

    public void CloseWindow() {
        questDetail.gameObject.SetActive(false);
    }
}
