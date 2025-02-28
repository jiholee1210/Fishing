using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set;}

    [SerializeField] PlayerActing playerActing;
    [SerializeField] UIManager uIManager;
    [SerializeField] GameObject npcWindow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    public void StartFishing(PlayerData playerData, PlayerInventory playerInventory) {
        uIManager.OpenFishingUI(playerData, playerInventory);
    }

    public void EndFishing() {
        playerActing.EndFishing();
    }

    public void OpenInventory() {
        uIManager.OpenInventoryUI();
    }

    public void CloseInventory() {
        uIManager.CloseInventoryUI();
    }

    public void OpenNPCUI(int npcType) {
        npcWindow.SetActive(true);
        switch(npcType) {
            case 1:
                uIManager.OpenMerchantUI();
                break;
        }
    }

    public void CloseNpcUI(int npcType) {
        npcWindow.SetActive(false);
        switch(npcType) {
            case 1:
                uIManager.CloseMerchantUI();
                break;
        }
    }
}
