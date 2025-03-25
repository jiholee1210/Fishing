using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNpcManager : MonoBehaviour
{
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] GameObject reqFishPrefab;

    [SerializeField] private Transform normalQuestParent;
    [SerializeField] private Transform epicQuestParent;
    [SerializeField] Transform reqParent;

    [SerializeField] TMP_Text talkText;

    [SerializeField] Transform questDetail;

    private List<QuestData> npcQuest;
    private List<int> completeQuest;

    private List<PlayerFish> playerFish;
    private PlayerData playerData;
    private Sprite[] gradeSprites;

    private GameObject npcObject;

    private int npcID;
    private int startID;

    private float nextQuestReset = 600f;
    private float curTime = 0f;
    
    [SerializeField] private PlayerInventory playerInventory;

    void Update()
    {
        curTime += Time.deltaTime;
        //10분마다 퀘스트 목록 초기화 시키는 기능
        if(curTime >= nextQuestReset) {
            startID = completeQuest.Count > 0 && completeQuest.Max() >= 1000 ? completeQuest.Max() + 1 : 1000;
            npcQuest.RemoveAll(item => !item.isEpic);
            CreateQuest(1);
            CreateQuest(2);
            CreateQuest(3);
            CreateQuestItems();
            questDetail.gameObject.SetActive(false);
            curTime = 0f;
            Debug.Log("노말 퀘스트 초기화");
            DataManager.Instance.SaveQuestNpcData();
        }
    }

    private void CreateQuest(int groundType) {
        
        for(int i = 0; i < 5; i++) {
            
            int questID =  startID++;
            int fishCount = Random.Range(1, 4);
            int price = 0;
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
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = questID;
            quest.isEpic = false;
            quest.questName = "마을의 부탁";
            quest.receive = groundType;
            quest.complete = groundType;
            quest.requirements = reqFish;
            quest.rewardGold = price;
            npcQuest.Add(quest);
            Debug.Log("노말 퀘스트 추가");
        }
    }

    private void SetDetail(QuestData questData, bool isNpc) {
        questDetail.gameObject.SetActive(true);
        questDetail.GetChild(5).gameObject.SetActive(false);
        questDetail.GetChild(6).gameObject.SetActive(false);
        
        TMP_Text name = questDetail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text desc = questDetail.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text gold = questDetail.GetChild(3).GetComponent<TMP_Text>();
        
        Button btn = questDetail.GetChild(5).GetComponent<Button>();
        questDetail.GetChild(5).gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => CompleteQuest(questData));
      
        name.text = questData.questName;
        desc.text = questData.desc;
        gold.text = questData.rewardGold + " 코인";

        foreach(Transform req in reqParent) {
            Destroy(req.gameObject);
        }

        int len = questData.requirements.Length;

        for(int i = 0; i < len; i++) {
            GameObject questReq = Instantiate(reqFishPrefab, reqParent);
            RectTransform rect = questReq.GetComponent<RectTransform>();

            questReq.transform.GetChild(0).GetComponent<Image>().sprite = gradeSprites[questData.requirements[i].grade];
            questReq.transform.GetChild(1).GetComponent<Image>().sprite = DataManager.Instance.GetFishData(questData.requirements[i].fishID).fishIcon;

            float xPos = (-90 * (len - 1)) + (160 * i);
            rect.anchoredPosition = new Vector2(xPos, 0);
        }
    }

    public void DefaultSetting() {
        completeQuest = DataManager.Instance.playerData.completeQuest;
        playerFish = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        npcQuest = DataManager.Instance.npcQuestList;
        gradeSprites = DataManager.Instance.gradeSprites;
    }

    public void CompleteQuest(QuestData questData) {
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
                return;
            }
        }
        foreach(int i in list) {
            playerFish[i] = null;
        }
        EventManager.Instance.ClearQuest(questData.questID);

        playerData.gold += questData.rewardGold;

        completeQuest.Add(questData.questID);
        npcQuest.Remove(questData);
        questDetail.gameObject.SetActive(false);
        foreach(var id in questData.nextQuest) {
            npcQuest.Add(DataManager.Instance.GetQuestData(id));
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
        int nCount = 0;
        int eCount = 0;
        // 새로운 퀘스트 아이템 생성
        Debug.Log("퀘스트 개수 : " + npcQuest.Count);
        for(int i = 0; i < npcQuest.Count; i++)
        {
            GameObject quest;
            int index = i;
            int count = 0;
            float yPos = 0f;
            // npc id와 퀘스트 완료 npc 아이디가 같을때만 출력하도록
            if(npcQuest[index].receive != npcID) continue;

            Debug.Log(npcQuest[index].questID);

            if(npcQuest[index].isEpic) {
                Debug.Log("에픽 퀘스트 생성");
                quest = Instantiate(questItemPrefab, epicQuestParent);
                count = eCount++;
                yPos = 230f;
            }
            else {
                Debug.Log("노말 퀘스트 생성");
                quest = Instantiate(questItemPrefab, normalQuestParent);
                count = nCount++;
                yPos = -115f;
            }
            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, yPos - (count * (rectTransform.rect.height + 10f)));
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = npcQuest[index].questName; 
            
            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(npcQuest[index], true));
        }
    }

    public void SetQuest() {
        CreateQuestItems();
    }

    public void SetTalk(GameObject _npcObject) {
        npcObject = _npcObject;
        npcID = npcObject.GetComponent<IQuest>().GetNpcID();
        Debug.Log("NPCManager" + npcObject);
        talkText.text = npcObject.GetComponent<INPC>().GetLine();
    }

    public void CloseWindow() {
        questDetail.gameObject.SetActive(false);
    }
}
