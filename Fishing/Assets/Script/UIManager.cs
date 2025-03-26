using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;

    [SerializeField] GameObject fishInventoryUI;
    [SerializeField] GameObject guideUI;
    [SerializeField] GameObject signUI;
    [SerializeField] GameObject optionUI;

    [SerializeField] GameObject upgradeNpcUI;
    [SerializeField] GameObject fishMerchantUI;
    [SerializeField] GameObject questNpcUI;
    [SerializeField] GameObject fishFarmNpcUI;

    [SerializeField] FishTradeManager fishTradeManager;
    [SerializeField] QuestNpcManager questNpcManager;
    [SerializeField] FishFarmManager fishFarmManager;
    [SerializeField] SignManager signManager;

    private GameObject npcObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishInventoryUI.GetComponent<FishInvenManager>().DefaultSetting();
        guideUI.GetComponent<GuideManager>().DefaultSetting();
        questNpcManager.DefaultSetting();
        fishTradeManager.DefaultSetting();
    }
    
    // 낚시 UI
    public void OpenFishingUI(List<FishData> fishList, List<int> itemList) {
        fishingUI.GetComponent<FishingManager>().StartFishing(fishList, itemList);
    }

    // 물고기 인벤토리 UI
    public void OpenFishInventoryUI() {
        fishInventoryUI.SetActive(true);
    }

    public void CloseFishInventoryUI() {
        fishInventoryUI.SetActive(false);
    }

    // 물고기 도감 UI
    public void OpenGuideUI() {
        guideUI.SetActive(true);
    }

    public void CloseGuideUI() {
        guideUI.SetActive(false);
    }

    // 장비 상인 대화
    public void OpenUpgradeNpcTalkUI() {
        upgradeNpcUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseUpgradeNpcTalkUI() {
        Debug.Log("상인 npc 창 닫음");
        upgradeNpcUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    // 물고기 상인 대화
    public void OpenFishMerchantTalkUI() {
        fishMerchantUI.SetActive(true);
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseFishMerchantTalkUI() {
        Debug.Log("물고기 npc 창 닫음");
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        fishMerchantUI.SetActive(false);
    }

    // 퀘스트 NPC 대화
    public void OpenQuestNpcTalkUI() {
        questNpcUI.transform.GetChild(0).gameObject.SetActive(true);
        questNpcManager.SetTalk(npcObject);
    }

    public void CloseQuestNpcTalkUI() {
        Debug.Log("퀘스트 npc 창 닫음");
        questNpcUI.transform.GetChild(0).gameObject.SetActive(false);
    }

    // 양식장 NPC 대화
    public void OpenFishFarmNpcUI() {
        fishFarmNpcUI.gameObject.SetActive(true);
        fishFarmManager.SetMainSlot();
    }

    public void CloseFishFarmNpcUI() {
        fishFarmNpcUI.gameObject.SetActive(false);
    }

    public void OpenUpgradeNpcUI() {
        upgradeNpcUI.transform.GetChild(0).gameObject.SetActive(false);
        upgradeNpcUI.transform.GetChild(1).gameObject.SetActive(true);
        upgradeNpcUI.GetComponent<UpgradeManager>().DefaultSetting();
    }

    public void CloseUpgradeNpcUI() {
        upgradeNpcUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    // 물고기 상인 거래
    public void OpenFishMerchantTradeUI() {
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CloseFishMerchantTradeUI() {
        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
    } 

    // 퀘스트 NPC 확인
    public void OpenQuestNpcCheckUI() {
        questNpcUI.transform.GetChild(0).gameObject.SetActive(false);
        questNpcUI.transform.GetChild(1).gameObject.SetActive(true);
        questNpcManager.SetQuest();
    }

    public void CloseQuestNpcCheckUI() {
        questNpcUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OpenSignUI() {
        signUI.SetActive(true);
        signManager.ShowFirstImage();
    }

    public void CloseSignUI() {
        signUI.SetActive(false);
    }

    public void OpenOptionUI() {
        optionUI.SetActive(true);
    }

    public void CloseOptionUI() {
        optionUI.SetActive(false);
    }

    public void CloseAllWindows() {
        fishInventoryUI.GetComponent<FishInvenManager>().CloseWindow();
        fishInventoryUI.SetActive(false);

        guideUI.GetComponent<GuideManager>().CloseWindow();
        guideUI.SetActive(false);

        upgradeNpcUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseUpgradeNpcTalkUI();

        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseFishMerchantTalkUI();

        questNpcManager.CloseWindow();
        questNpcUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseQuestNpcTalkUI();

        fishFarmManager.CloseWindow();
        CloseFishFarmNpcUI();

        CloseSignUI();

        CloseOptionUI();
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }
}
