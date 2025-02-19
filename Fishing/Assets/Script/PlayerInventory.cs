using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Inventory inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetFish(int fishID) {
        PlayerFish fish = inventory.fishList.Find(f => f.fishID == fishID);
        if (fish != null) {
            fish.catchCount++;
        }
        else {
            inventory.fishList.Add(new PlayerFish{
                fishID = fishID,
                fishName = DataManager.Instance.GetFishNameFromList(fishID),
                catchCount = 1
            });
        }
        DataManager.Instance.SaveInventoryData();
    }

    public void SetInventory() {
        inventory = DataManager.Instance.inventory;
    } 
}
