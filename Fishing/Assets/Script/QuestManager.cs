using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField] GameObject questItemPrefab;
    [SerializeField] GameObject reqFishPrefab;
    [SerializeField] GameObject rewardItemPrefab;

    [SerializeField] Transform questParent;  // Vertical Layout Group이 있는 부모 오브젝트
    [SerializeField] Transform reqParent;
    [SerializeField] Transform rewardParent;

    [SerializeField] Transform questDetail;

    private List<QuestData> questDatas;
    // 플레이어 데이터에서 퀘스트 리스트 받아와서 아이템 리스트 오브젝트 하위에 나열
    // 아이템 클릭 시 디테일 창에 정보 출력 및 요구 물고기 프리팹, 보상 아이템 프리팹으로 나열

    private void SetDetail(QuestData questData) {
        questDetail.gameObject.SetActive(true);
        
        TMP_Text name = questDetail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text desc = questDetail.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text gold = questDetail.GetChild(3).GetComponent<TMP_Text>();

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
        questDatas = DataManager.Instance.playerQuestList;
    }

    private void CreateQuestItems()
    {
        // 기존 퀘스트 아이템들 제거
        foreach(Transform child in questParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 퀘스트 아이템 생성
        for(int i = 0; i < questDatas.Count; i++)
        {
            int index = i;
            GameObject quest = Instantiate(questItemPrefab, questParent);
            RectTransform rectTransform = quest.GetComponent<RectTransform>();
            
            // 세로 위치 계산 (위에서부터 아래로)
            float yPosition = 230f - (index * (rectTransform.rect.height + 20f));
            rectTransform.anchoredPosition = new Vector2(0, yPosition);

            // 퀘스트 데이터 설정
            rectTransform.GetChild(0).GetComponent<TMP_Text>().text = questDatas[index].questName; 
            quest.GetComponent<Quest>().SetQuest(questDatas[index]);
            quest.GetComponent<Button>().onClick.AddListener(() => SetDetail(questDatas[index]));
        }
    }

    private void OnEnable() {
        CreateQuestItems();
    }

    void OnDisable()
    {
        questDetail.gameObject.SetActive(false);
    }

    public void CloseWindow() {
        questDetail.gameObject.SetActive(false);
    }
}
