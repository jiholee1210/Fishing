using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] FishInvenManager fishInvenManager;

    private Inventory inventory;
    private PlayerActing playerActing;

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
            GetEquip(0);
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            GetEquip(1);
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            GetEquip(10);
            GetEquip(20);
            GetEquip(30);
            GetEquip(40);
        }
    
    }

    public void GetFish(int fishID, float _weight) {
        FishData fishData = DataManager.Instance.GetFishData(fishID);
        for(int i = 0; i < inventory.fishList.Count; i++) {
            if(inventory.fishList[i].fishID == 0) {
                inventory.fishList[i] = new PlayerFish{
                    fishID = fishID,
                    weight = _weight,
                    price = (int)(fishData.price * (_weight / fishData.weightMin))
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

    public bool isFishFull() {
        return inventory.fishList.All(fish => fish.fishID != 0);
    }

    public bool NotEquip() {
        return inventory.equip.Any(item => item == null);
    }

    public void SetSlots(List<ItemData> _slots) {
        inventory.slots = _slots;
    }

    public void SetEquip() {
        StartCoroutine(playerActing.SetAnimator());
    }
    public bool haveRod() {
        return inventory.equip[0] != null ? true : false;
    }

    public bool haveBait() {
        return inventory.equip[4] != null ? true : false;
    }

    public float GetRodDur() {
        float rodDur = inventory.equip[0] != null ? DataManager.Instance.GetRodData(inventory.equip[0].itemID).rodDur : 0;
        Debug.Log("플레이어 낚시 파워 : " + rodDur);
        return rodDur;
    }

    public float GetReelSpeed() {
        float reelSpeed = inventory.equip[1] != null ? DataManager.Instance.GetReelData(inventory.equip[1].itemID).reelSpeed : 0;
        return reelSpeed;
    }

    public float GetWireDur() {
        float wireDur = inventory.equip[2] != null ? DataManager.Instance.GetWireData(inventory.equip[2].itemID).wireDur : 0;
        return wireDur;
    }

    public float GetHookPower() {
        float hookPower = inventory.equip[3] != null ? DataManager.Instance.GetHookData(inventory.equip[3].itemID).hookPower : 0;
        return hookPower;
    }

    public int GetBaitLevel() {
        int baitLevel = inventory.equip[4] != null ? DataManager.Instance.GetBaitData(inventory.equip[4].itemID).baitLevel : 0;
        return baitLevel;
    }

}
