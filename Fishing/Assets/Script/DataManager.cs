using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set;}
    
    string playerPath;
    string inventoryPath;
    public PlayerData playerData;
    public Inventory inventory;

    public Dictionary<int, FishData> fishDataDict;
    public Dictionary<int, RodData> rodDataDict;
    public Dictionary<int, ItemData> itemDataDict;

    void Awake()
    {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        Init();
    }

    private void Init() {
        playerPath = Path.Combine(Application.persistentDataPath, "playerdata.json");
        inventoryPath = Path.Combine(Application.persistentDataPath, "inventory.json");
        if(!File.Exists(playerPath)) {
            playerData = new();
            SavePlayerData();
            Debug.Log("데이터 새로 생성");
        }
        else {
            LoadPlayerData();
        }
        
        if(!File.Exists(inventoryPath)) {
            inventory = new();
            SaveInventoryData();
            Debug.Log("인벤토리 생성");
        }
        else {
            LoadInventoryData();
        }
        LoadFishDataFromSo();
        LoadRodDataFromSo();
        LoadItemDataFromSo();
    }

    private void LoadFishDataFromSo() {
        FishData[] fishDataArray = Resources.LoadAll<FishData>("FishData");
        fishDataDict = new Dictionary<int, FishData>();
        foreach(FishData fish in fishDataArray) {
            fishDataDict[fish.fishID] = fish;
        }
        Debug.Log("물고기 데이터 불러오기");
    }

    private void LoadRodDataFromSo() {
        RodData[] rodDataArray = Resources.LoadAll<RodData>("RodData");
        rodDataDict = new Dictionary<int, RodData>();
        foreach(RodData rod in rodDataArray) {
            rodDataDict[rod.rodID] = rod;
        }
        Debug.Log("낚싯대 데이터 불러오기");
    }
    
    private void LoadItemDataFromSo() {
        ItemData[] itemDataArray = Resources.LoadAll<ItemData>("ItemData");
        itemDataDict = new Dictionary<int, ItemData>();
        foreach(ItemData item in itemDataArray) {
            itemDataDict[item.itemID] = item;
        }
        Debug.Log("아이템 데이터 불러오기");
    }

    public void SavePlayerData() {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerPath, json);
        Debug.Log("데이터 저장");
    }

    public void LoadPlayerData() {
        string json = File.ReadAllText(playerPath);
        playerData = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("데이터 로드");
    }

    public void SaveInventoryData() {
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(inventoryPath, json);
        Debug.Log("인벤토리 저장");
    }

    public void LoadInventoryData() {
        string json = File.ReadAllText(inventoryPath);
        inventory = JsonUtility.FromJson<Inventory>(json);
        Debug.Log("인벤토리 로드");
    }

    public string GetFishNameFromList(int id) {
        return fishDataDict.TryGetValue(id, out FishData fish) ? fish.fishName : "";
    }

    public List<int> GetFishIDFromList(int rarity) {
        List<int> IDList = new();
        foreach (var fishData in fishDataDict) {
            if(fishData.Value.rarity == rarity) {
                IDList.Add(fishData.Key);
            }
        }
        return IDList;
    }

    public float GetFishPowerFromList(int id) {
        return fishDataDict.TryGetValue(id, out FishData fish) ? fish.power : 0;
    }

    public float GetRodPowerFromList(int id) {
        return rodDataDict.TryGetValue(id, out RodData rod) ? rod.rodPower : 0;
    }

    public RodData GetRodData(int id) {
        return rodDataDict.TryGetValue(id, out RodData rod) ? rod : null;
    }

    public FishData GetFishData(int id) {
        return fishDataDict.TryGetValue(id, out FishData fish) ? fish : null;
    }

    public ItemData GetItemData(int id) {
        return itemDataDict.TryGetValue(id, out ItemData item) ? item : null;
    }
 
}

[System.Serializable]
public class PlayerData {
    // 스테미나, 인벤토리
    public float stamina = 10f;
    public int gold = 0;
}

[System.Serializable]
public class Inventory {
    public List<PlayerFish> fishList = new();
    public List<ItemData> slots = new(new ItemData[54]);
    public List<ItemData> equip = new(new ItemData[5]);
}

[System.Serializable]
public class PlayerFish {
    public string fishName;
    public int fishID;
    public int catchCount;
}
