using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject merchantUI;
    [SerializeField] TradeManager tradeManager;

    private GameObject npcObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    public void OpenMerchantTalkUI() {
        merchantUI.SetActive(true);
        merchantUI.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseMerchantTalkUI() {
        merchantUI.transform.GetChild(0).gameObject.SetActive(false);
        merchantUI.SetActive(false);
    }

    public void OpenMerchantTradeUI() {
        merchantUI.transform.GetChild(0).gameObject.SetActive(false);
        merchantUI.transform.GetChild(1).gameObject.SetActive(true);
        tradeManager.SetNpcObject(npcObject);
    }

    public void CloseMerchantTradeUI() {
        merchantUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CloseAllWindows() {
        inventoryUI.GetComponent<InventoryManager>().CloseWindow();
        inventoryUI.SetActive(false);

        tradeManager.CloseWindow();
        merchantUI.transform.GetChild(1).gameObject.SetActive(false);

        merchantUI.transform.GetChild(0).gameObject.SetActive(false);
        merchantUI.SetActive(false);
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }
}
