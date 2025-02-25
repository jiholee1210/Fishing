using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set;}

    [SerializeField] PlayerActing playerActing;
    [SerializeField] UIManager uIManager;
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
}
