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
            GetEquip(1);
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            GetEquip(2);
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            GetEquip(11);
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

    public bool isFishFull() {
        return inventory.fishList.All(fish => fish.fishID != 0);
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

    public float GetRodDur() {
        int rodPower = inventory.equip[0] != null ? DataManager.Instance.GetRodData(inventory.equip[0].itemID).rodDur : 0;
        Debug.Log("플레이어 낚시 파워 : " + rodPower);
        return rodPower;
    }

    public float GetReelPower() {
        float reelPower = inventory.equip[1] != null ? DataManager.Instance.GetReelData(inventory.equip[1].itemID).reelPower : 0;
        return reelPower;
    }

}
