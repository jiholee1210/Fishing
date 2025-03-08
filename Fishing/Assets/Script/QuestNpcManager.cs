using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNpcManager : MonoBehaviour
{
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Transform npcQuestParent;  // Vertical Layout Group이 있는 부모 오브젝트
    [SerializeField] private Transform playerQuestParent;

    [SerializeField] GameObject reqFishPrefab;
    [SerializeField] GameObject rewardItemPrefab;

    [SerializeField] Transform questDetail;

    private List<QuestData> playerQuest;
    private List<QuestData> npcQuest;

    private void SetDetail(QuestData questData, bool isNpc) {
        questDetail.gameObject.SetActive(true);
        
        TMP_Text name = questDetail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text desc = questDetail.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text gold = questDetail.GetChild(3).GetComponent<TMP_Text>();
        Button btn = questDetail.GetChild(5).GetComponent<Button>();
        if(isNpc) {
            questDetail.GetChild(5).gameObject.SetActive(true);
        }
        else {
            questDetail.GetChild(5).gameObject.SetActive(false);
        }
        

        name.text = questData.questName;
        desc.text = questData.desc;
        gold.text = questData.rewardGold + " 코인";

        btn.onClick.AddListener(() => AddQuest(questData));
    }

    public void DefaultSetting() {
        playerQuest = DataManager.Instance.playerData.questList;
        npcQuest = DataManager.Instance.questNpc.questlist;
    }

    public void AddQuest(QuestData questData) {
        playerQuest.Add(questData);
        npcQuest.Remove(questData);
        questDetail.gameObject.SetActive(false);
        CreateQuestItems();
        DataManager.Instance.SavePlayerData();
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

    private void OnEnable() {
        CreateQuestItems();
    }

    void OnDisable()
    {
        questDetail.gameObject.SetActive(false);
    }
}
