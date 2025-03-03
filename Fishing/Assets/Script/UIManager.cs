using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject fishInventoryUI;
    [SerializeField] GameObject equipMerchantUI;
    [SerializeField] GameObject fishMerchantUI;
    [SerializeField] TradeManager tradeManager;
    [SerializeField] FishTradeManager fishTradeManager;

    private GameObject npcObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryUI.GetComponent<InventoryManager>().DefaultSetting();
        fishInventoryUI.GetComponent<FishInvenManager>().DefaultSetting();
        fishTradeManager.DefaultSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFishingUI(PlayerData playerData, PlayerInventory playerInventory) {
        fishingUI.SetActive(true);
        fishingUI.GetComponent<FishingManager>().ResetStatus(playerData, playerInventory);
    }

    public void OpenInventoryUI() {
        inventoryUI.SetActive(true);
    }

    public void CloseInventoryUI() {
        inventoryUI.SetActive(false);
    }

    public void OpenFishInventoryUI() {
        fishInventoryUI.SetActive(true);
    }

    public void CloseFishInventoryUI() {
        fishInventoryUI.SetActive(false);
    }

    public void OpenEquipMerchantTalkUI() {
        equipMerchantUI.SetActive(true);
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseEquipMerchantTalkUI() {
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        equipMerchantUI.SetActive(false);
    }

    public void OpenFishMerchantTalkUI() {
        fishMerchantUI.SetActive(true);
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseFishMerchantTalkUI() {
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        fishMerchantUI.SetActive(false);
    }

    public void OpenMerchantTradeUI() {
        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(true);
        tradeManager.SetNpcObject(npcObject);
    }

    public void CloseMerchantTradeUI() {
        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OpenFishMerchantTradeUI() {
        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CloseFishMerchantTradeUI() {
        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(false);
    } 

    public void CloseAllWindows() {
        inventoryUI.GetComponent<InventoryManager>().CloseWindow();
        inventoryUI.SetActive(false);

        fishInventoryUI.GetComponent<FishInvenManager>().CloseWindow();
        fishInventoryUI.SetActive(false);

        tradeManager.CloseWindow();
        equipMerchantUI.transform.GetChild(1).gameObject.SetActive(false);

        equipMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        equipMerchantUI.SetActive(false);

        fishTradeManager.CloseWindow();
        fishMerchantUI.transform.GetChild(1).gameObject.SetActive(false);

        fishMerchantUI.transform.GetChild(0).gameObject.SetActive(false);
        fishMerchantUI.SetActive(false);
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }
}
