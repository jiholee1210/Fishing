using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] FishInvenManager fishInvenManager;

    private Inventory inventory;
    private List<ItemData> slot;
    private List<ItemData> equip;
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
        slot = DataManager.Instance.slotList;
        equip = DataManager.Instance.equipList;
    }

    public bool isFishFull() {
        return inventory.fishList.All(fish => fish.fishID != 0);
    }

    public void SetEquip() {
        StartCoroutine(playerActing.SetAnimator());
    }

    public bool HaveRod() {
        return equip[0] != null;
    }

    public bool HaveBait() {
        return equip[4] != null;
    }

    public float GetRodDur() {
        float rodDur = equip[0] != null ? DataManager.Instance.GetRodData(equip[0].itemID).rodDur : 0;
        Debug.Log("플레이어 낚시 파워 : " + rodDur);
        return rodDur;
    }

    public float GetReelSpeed() {
        float reelSpeed = equip[1] != null ? DataManager.Instance.GetReelData(equip[1].itemID).reelSpeed : 0;
        return reelSpeed;
    }

    public float GetWirePower() {
        float wireDur = equip[2] != null ? DataManager.Instance.GetWireData(equip[2].itemID).wirePower : 0;
        return wireDur;
    }

    public float GetHookPower() {
        float hookPower = equip[3] != null ? DataManager.Instance.GetHookData(equip[3].itemID).hookPower : 0;
        return hookPower;
    }

    public int GetBaitLevel() {
        int baitLevel = equip[4] != null ? DataManager.Instance.GetBaitData(equip[4].itemID).baitLevel : 0;
        return baitLevel;
    }

}
