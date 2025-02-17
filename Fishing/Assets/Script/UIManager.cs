using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set;}

    [SerializeField] GameObject fishingUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFishingUI(PlayerInventory playerInventory) {
        fishingUI.SetActive(true);
        playerInventory.GetFish(1);
    }
}
