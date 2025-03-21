using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;

    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject fishInventoryUI;
    [SerializeField] GameObject questUI;
    [SerializeField] GameObject guideUI;
    [SerializeField] GameObject signUI;

    [SerializeField] GameObject equipMerchantUI;
    [SerializeField] GameObject fishMerchantUI;
    [SerializeField] GameObject questNpcUI;

    [SerializeField] TradeManager tradeManager;
    [SerializeField] FishTradeManager fishTradeManager;
    [SerializeField] QuestNpcManager questNpcManager;
    [SerializeField] SignManager signManager;

    private GameObject npcObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryUI.GetComponent<InventoryManager>().DefaultSetting();
        fishInventoryUI.GetComponent<FishInvenManager>().DefaultSetting();
        questUI.GetComponent<QuestManager>().DefaultSetting();
        guideUI.GetComponent<GuideManager>().DefaultSetting();
        questNpcManager.DefaultSetting();
        fishTradeManager.DefaultSetting();
    }
    
    // 낚시 UI
    public void OpenFishingUI(List<FishData> fishList) {
        fishingUI.GetComponent<FishingManager>().StartFishing(fishList);
    }

    // 인벤토리 UI
    public void OpenInventoryUI() {
        inventoryUI.SetActive(true);
    }

    public void CloseInventoryUI() {
        inventoryUI.SetActive(false);
    }

    // 물고기 인벤토리 UI
    public void OpenFishInventoryUI() {
        fishInventoryUI.SetActive(true);
    }

    public void CloseFishInventoryUI() {
        fishInventoryUI.SetActive(false);
    }

    // 퀘스트 UI
    public void OpenQuestUI() {
        questUI.SetActive(true);
    }

    public void CloseQuestUI() {
        questUI.SetActive(false);
    }

    // 물고기 도감 UI
    public void OpenGuideUI() {
        guideUI.SetActive(true);
    }

    public void CloseGuideUI() {
        guideUI.SetActive(false);
    }

    // 장비 상인 대화
    public void OpenEquipMerchantTalkUI() {
        equipMerchantUI.SetActive(true);
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseEquipMerchantTalkUI() {
        Debug.Log("상인 npc 창 닫음");
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        equipMerchantUI.SetActive(false);
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

    // 장비 상인 거래
    public void OpenMerchantTradeUI() {
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(true);
        Debug.Log("UIManager" + npcObject);
        tradeManager.SetNpcObject(npcObject);
    }

    public void CloseMerchantTradeUI() {
        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
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

    public void CloseAllWindows() {
        inventoryUI.GetComponent<InventoryManager>().CloseWindow();
        inventoryUI.SetActive(false);

        fishInventoryUI.GetComponent<FishInvenManager>().CloseWindow();
        fishInventoryUI.SetActive(false);

        questUI.GetComponent<QuestManager>().CloseWindow();
        questUI.SetActive(false);

        guideUI.GetComponent<GuideManager>().CloseWindow();
        guideUI.SetActive(false);

        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseEquipMerchantTalkUI();

        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseFishMerchantTalkUI();

        questNpcManager.CloseWindow();
        questNpcUI.transform.GetChild(1).gameObject.SetActive(false);
        CloseQuestNpcTalkUI();

        CloseSignUI();
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }
}
