using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject fishingUI;


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
}
