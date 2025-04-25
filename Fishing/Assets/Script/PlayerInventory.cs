using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] Transform handPos;

    private Inventory inventory;
    private PlayerActing playerActing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await DataManager.Instance.WaitForRodData();
        playerActing = GetComponent<PlayerActing>();
        ResetInventory();
        SetEquip();
    }

    public void GetFish(int _fishID, float _weight, int _grade) {
        FishData fishData = DataManager.Instance.GetFishData(_fishID);
        for(int i = 0; i < inventory.fishList.Count; i++) {
            if(inventory.fishList[i].fishID == -1) {
                inventory.fishList[i] = new PlayerFish{
                    fishID = _fishID,
                    weight = _weight,
                    grade = _grade,
                    price = (int)(fishData.price * (_weight / fishData.weightMin) * (_grade + 1))
                };
                break;
            }
        }
        DataManager.Instance.SaveInventoryData();
    }

    public void GetFish(PlayerFish playerFish) {
        for(int i = 0; i < inventory.fishList.Count; i++) {
            if(inventory.fishList[i].fishID == -1) {
                inventory.fishList[i] = playerFish.Clone();
                break;
            }   
        }
    }

    public void ResetInventory() {
        inventory = DataManager.Instance.inventory;
    }

    public bool isFishFull() {
        return inventory.fishList.All(fish => fish.fishID != -1);
    }

    public void SetEquip() {
        int rodID = DataManager.Instance.playerData.curRod;
        GameObject prefab = DataManager.Instance.GetRodData(rodID).rodPrefab;
        if(handPos.childCount > 0) {
            Destroy(handPos.GetChild(0).gameObject);
        }
        Instantiate(prefab, handPos);
        StartCoroutine(playerActing.SetAnimator());
    }

    public float GetRodDur() {
        float rodDur = DataManager.Instance.GetRodData(inventory.equip[0]).rodDur;
        return rodDur;
    }

    public float GetReelSpeed() {
        float reelSpeed = DataManager.Instance.GetReelData(10 + inventory.equip[1]).reelSpeed;
        return reelSpeed;
    }

    public float GetWirePower() {
        float wireDur = DataManager.Instance.GetWireData(20 + inventory.equip[2]).wirePower;
        return wireDur;
    }

    public float GetHookPower() {
        float hookPower = DataManager.Instance.GetHookData(30 + inventory.equip[3]).hookPower;
        return hookPower;
    }

    public int GetBaitLevel() {
        int baitLevel = DataManager.Instance.GetBaitData(40 + inventory.equip[4]).baitLevel;
        return baitLevel;
    }

}
