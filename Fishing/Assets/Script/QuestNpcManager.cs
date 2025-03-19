using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNpcManager : MonoBehaviour
{
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] GameObject reqFishPrefab;
    [SerializeField] GameObject rewardItemPrefab;

    [SerializeField] private Transform npcQuestParent;
    [SerializeField] private Transform playerQuestParent;
    [SerializeField] Transform reqParent;
    [SerializeField] Transform rewardParent;

    [SerializeField] TMP_Text talkText;

    [SerializeField] Transform questDetail;

    private List<QuestData> npcQuest;
    private List<QuestData> playerQuest;
    private List<PlayerFish> playerFish;
    private PlayerData playerData;

    private GameObject npcObject;
    
    [SerializeField] private PlayerInventory playerInventory;

    private void SetDetail(QuestData questData, bool isNpc) {
        questDetail.gameObject.SetActive(true);
        questDetail.GetChild(6).gameObject.SetActive(false);
        questDetail.GetChild(7).gameObject.SetActive(false);
        
        TMP_Text name = questDetail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text desc = questDetail.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text gold = questDetail.GetChild(3).GetComponent<TMP_Text>();
        
        if(isNpc) {
            Button btn = questDetail.GetChild(6).GetComponent<Button>();
            questDetail.GetChild(6).gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => AddQuest(questData));
        }
        else {
            Button btn = questDetail.GetChild(7).GetComponent<Button>();
            questDetail.GetChild(7).gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CompleteQuest(questData));
        }

        name.text = questData.questName;
        desc.text = questData.desc;
        gold.text = questData.rewardGold + " 코인";

        foreach(Transform req in reqParent) {
            Destroy(req.gameObject);
        }

        foreach(Transform reward in rewardParent) {
            Destroy(reward.gameObject);
        }

        int len = questData.requirements.Length;

        for(int i = 0; i < len; i++) {
            GameObject questReq = Instantiate(reqFishPrefab, reqParent);
            RectTransform rect = questReq.GetComponent<RectTransform>();

            questReq.GetComponent<Image>().sprite = DataManager.Instance.GetFishData(questData.requirements[i].fishID).fishIcon;
            questReq.GetComponent<Transform>().GetChild(0).GetComponent<TMP_Text>().text = questData.requirements[i].weight.ToString() + " kg";

            float xPos = (-60 * (len - 1)) + (120 * i);
            rect.anchoredPosition = new Vector2(xPos, 0);
        }

        len = questData.rewardItem.Length;
        for(int i = 0; i < len; i++) {
            GameObject questReward = Instantiate(rewardItemPrefab, rewardParent);
            RectTransform rect = questReward.GetComponent<RectTransform>();

            questReward.GetComponent<Image>().sprite = DataManager.Instance.GetItemData(questData.rewardItem[i]).itemImage;

            float xPos = (-60 * (len - 1)) + (120 * i);
            rect.anchoredPosition = new Vector2(xPos, 0);
        }
    }

    public void DefaultSetting() {
        playerQuest = DataManager.Instance.playerQuest;
        playerFish = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
    }

    public void AddQuest(QuestData questData) {
        playerQuest.Add(questData);
        npcQuest.Remove(questData);
        questDetail.gameObject.SetActive(false);
        CreateQuestItems();
        DataManager.Instance.SavePlayerData();
        DataManager.Instance.SaveQuestNpcData();
    }

    public void CompleteQuest(QuestData questData) {
        List<PlayerFish> available = new(playerFish);
        List<int> list = new();

        foreach(var requirement in questData.requirements) {
            bool fishFound = false;
            for(int i = 0; i < available.Count; i++) {
                if(available[i].fishID == requirement.fishID && available[i].weight >= requirement.weight) {
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

        foreach(var reward in questData.rewardItem) {
            playerInventory.GetEquip(reward);
        }

        playerData.gold += questData.rewardGold;

        playerQuest.Remove(questData);
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
        foreach(Transform child in npcQuestParent)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in playerQuestParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 퀘스트 아이템 생성
        for(int i = 0; i < npcQuest.Count; i++)
        {
            int index = i;
            GameObject quest = Instantiate(questItemPrefab, npcQuestParent);
            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            
            // 세로 위치 계산 (위에서부터 아래로)
            float yPosition = 230f - (index * (rectTransform.rect.height + 20f));
            rectTransform.anchoredPosition = new Vector2(0, yPosition);

            // 퀘스트 데이터 설정
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = npcQuest[index].questName; 
            quest.GetComponent<Quest>().SetQuest(npcQuest[index]);
            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(npcQuest[index], true));
        }

        for(int i = 0; i < playerQuest.Count; i++)
        {
            int index = i;
            GameObject quest = Instantiate(questItemPrefab, playerQuestParent);
            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            
            // 세로 위치 계산 (위에서부터 아래로)
            float yPosition = -115f - (index * (rectTransform.rect.height + 10f));
            rectTransform.anchoredPosition = new Vector2(0, yPosition);

            // 퀘스트 데이터 설정
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = playerQuest[index].questName; 
            quest.GetComponent<Quest>().SetQuest(playerQuest[index]);
            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(playerQuest[index], false));
        }
    }

    public void SetQuest() {
        npcQuest = npcObject.GetComponent<IQuest>().GetQuestList();
        Debug.Log(npcQuest.Count);
        CreateQuestItems();
    }

    public void SetTalk(GameObject _npcObject) {
        npcObject = _npcObject;
        Debug.Log("NPCManager" + npcObject);
        talkText.text = npcObject.GetComponent<INPC>().GetLine();
    }

    public void CloseWindow() {
        questDetail.gameObject.SetActive(false);
    }
}
