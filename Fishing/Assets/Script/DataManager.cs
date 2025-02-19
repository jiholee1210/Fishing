using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set;}
    
    string playerPath;
    string inventoryPath;
    public PlayerData playerData;
    public Inventory inventory;

    public Dictionary<int, FishData> fishDataDict;

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
    }

    private void LoadFishDataFromSo() {
        FishData[] fishDataArray = Resources.LoadAll<FishData>("FishData");
        fishDataDict = new Dictionary<int, FishData>();
        foreach(FishData fish in fishDataArray) {
            fishDataDict[fish.fishID] = fish;
        }
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
    public List<int> equip = new();
    public int rod;
    public int reel;
    public int wire;
    public int hook;
    public int bait;
}

[System.Serializable]
public class PlayerFish {
    public string fishName;
    public int fishID;
    public int catchCount;
}
