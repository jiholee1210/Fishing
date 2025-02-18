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

    public void StartFishing(PlayerInventory playerInventory) {
        uIManager.OpenFishingUI(playerInventory);
    }

    public void EndFishing() {
        playerActing.EndFishing();
    }
}
