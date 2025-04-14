using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;

    [SerializeField] GameObject fishInventoryUI;
    [SerializeField] GameObject guideUI;
    [SerializeField] private GameObject skinUI;
    [SerializeField] GameObject signUI;
    [SerializeField] GameObject optionUI;
    [SerializeField] private GameObject volumeUI;

    [SerializeField] private Animator inventoryFullError;
    [SerializeField] private Animator notClearQuestError;

    [SerializeField] GameObject upgradeNpcUI;
    [SerializeField] GameObject fishMerchantUI;
    [SerializeField] GameObject questNpcUI;
    [SerializeField] GameObject fishFarmNpcUI;
    [SerializeField] GameObject museumNpcUI;

    [SerializeField] FishTradeManager fishTradeManager;
    [SerializeField] QuestNpcManager questNpcManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] FishFarmManager fishFarmManager;
    [SerializeField] MuseumManager museumManager;
    [SerializeField] SignManager signManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private GuideManager guideManager;
    [SerializeField] private FishInvenManager fishInvenManager;
    [SerializeField] private OptionManager optionManager;

    private GameObject npcObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // 낚시 UI
    public void OpenFishingUI(List<FishData> fishList) {
        fishingUI.GetComponent<FishingManager>().StartFishing(fishList);
    }

    // 물고기 인벤토리 UI
    public void OpenFishInventoryUI() {
        fishInventoryUI.SetActive(true);
        fishInvenManager.DefaultSetting();
    }

    public void CloseFishInventoryUI() {
        fishInventoryUI.SetActive(false);
        fishInvenManager.CloseWindow();
    }

    // 물고기 도감 UI
    public void OpenGuideUI() {
        guideUI.SetActive(true);
    }

    public void CloseGuideUI() {
        guideUI.SetActive(false);
        guideManager.CloseWindow();
    }

    // 스킨 변경
    public void OpenSkinUI() {
        skinUI.SetActive(true);
        skinManager.SetSlots();
    }

    public void CloseSkinUI() {
        skinUI.SetActive(false);
        skinManager.CloseWindow();
    }

    // 양식장 NPC
    public void OpenFishFarmNpcUI() {
        fishFarmNpcUI.SetActive(true);
        fishFarmManager.SetMainSlot();
    }

    public void CloseFishFarmNpcUI() {
        fishFarmNpcUI.SetActive(false);
        fishFarmManager.CloseWindow();
    }

    // 박물관 NPC
    public void OpenMuseumNpcUI() {
        museumNpcUI.SetActive(true);
    }

    public void CloseMuseumNpcUI() {
        museumNpcUI.SetActive(false);
        museumManager.CloseWindow();
    }

    // 업그레이드 NPC
    public void OpenUpgradeNpcUI() {
        upgradeNpcUI.SetActive(true);
        upgradeManager.DefaultSetting();
    }

    public void CloseUpgradeNpcUI() {
        upgradeNpcUI.SetActive(false);
        upgradeManager.CloseWindow();
    }

    // 물고기 거래 NPC
    public void OpenFishMerchantUI() {
        fishMerchantUI.SetActive(true);
        fishTradeManager.DefaultSetting();
    }

    public void CloseFishMerchantUI() {
        fishMerchantUI.SetActive(false);
        fishTradeManager.CloseWindow();
    } 

    // 퀘스트 NPC
    public void OpenQuestNpcUI() {
        questNpcUI.SetActive(true);
        questNpcManager.SetQuest(npcObject);
    }

    public void CloseQuestNpcUI() {
        questNpcUI.SetActive(false);
        questNpcManager.CloseWindow();
    }

    // 표지판
    public void OpenSignUI() {
        signUI.SetActive(true);
        signManager.ShowFirstImage();
    }

    public void CloseSignUI() {
        signUI.SetActive(false);
    }

    // 옵션
    public void OpenOptionUI() {
        optionUI.SetActive(true);
    }

    public void CloseOptionUI() {
        optionManager.CloseWindow();
    }

    public void InventoryFull() {
        inventoryFullError.Play("InventoryFullError");
    }

    public void NotClearQuest() {
        notClearQuestError.Play("NotClearQuestError");
    }

    public void CloseAllWindows() {
        SoundManager.Instance.OpenUI();
        CloseFishInventoryUI();

        CloseGuideUI();

        CloseSkinUI();

        CloseUpgradeNpcUI();

        CloseFishMerchantUI();

        CloseQuestNpcUI();

        CloseFishFarmNpcUI();

        CloseMuseumNpcUI();

        CloseSignUI();

        CloseOptionUI();
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }
}
