using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;

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
        inventoryManager.AddItemToSlot(itemID);
    }

    public void ResetInventory() {
        inventory = DataManager.Instance.inventory;
    }

    public void SetSlots(List<ItemData> _slots) {
        inventory.slots = _slots;
    }

    public void SetEquip(List<ItemData> _equip) {
        inventory.equip = _equip;
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
