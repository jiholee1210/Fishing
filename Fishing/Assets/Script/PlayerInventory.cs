using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    private Inventory inventory;

    private float rodPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetInventory();
        SetRodPower();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) {
            GetEquip(1);
        }
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

    public void GetEquip(int itemID) {
        inventory.equip.Add(itemID);
        inventoryManager.AddEquipToSlot(itemID);
    }

    public void SetInventory() {
        inventory = DataManager.Instance.inventory;
    } 

    public void SetRodPower() {
        rodPower = DataManager.Instance.GetRodPowerFromList(inventory.rod);
    }

    public float GetRodPower() {
        Debug.Log("플레이어 낚시 파워 : " + rodPower);
        return rodPower;
    }
}
