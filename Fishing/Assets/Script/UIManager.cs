using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;
    [SerializeField] GameObject inventoryUI;

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
}
