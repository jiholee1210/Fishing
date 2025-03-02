using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] FishInvenManager fishInvenManager;

    private Inventory inventory;
    private PlayerActing playerActing;

    private float rodPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerActing = GetComponent<PlayerActing>();
        ResetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) {
            GetEquip(1);
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            GetEquip(2);
        }
    }

    public void GetFish(int fishID) {
        FishData fishData = DataManager.Instance.GetFishData(fishID);
        float randomWeight = Random.Range(fishData.weightMin, fishData.weightMax);
        float weight = float.Parse(randomWeight.ToString("F2"));
        for(int i = 0; i < inventory.fishList.Count; i++) {
            if(inventory.fishList[i].fishID == 0) {
                inventory.fishList[i] = new PlayerFish{
                    fishID = fishID,
                    weight = weight,
                    price = (int)(fishData.price * (weight / fishData.weightMin))
                };
                break;
            }
        }
        DataManager.Instance.SaveInventoryData();
    }

    public void GetEquip(int itemID) {
        inventoryManager.AddItemToSlot(itemID);
    }

    public void ResetInventory() {
        inventory = DataManager.Instance.inventory;
    }

    public void SetSlots(List<ItemData> _slots) {
        inventory.slots = _slots;
    }

    public void SetEquip() {
        StartCoroutine(playerActing.SetAnimator());
    }

    public void SetRodPower() {
        rodPower = inventory.equip[0] != null ? DataManager.Instance.GetRodPowerFromList(inventory.equip[0].itemID) : 0;
    }

    public bool haveRod() {
        return inventory.equip[0] != null ? true : false;
    }

    public float GetRodPower() {
        Debug.Log("플레이어 낚시 파워 : " + rodPower);
        return rodPower;
    }
}
