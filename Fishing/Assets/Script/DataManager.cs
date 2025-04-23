using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set;}

    [SerializeField] public Sprite[] gradeSprites;
    
    string playerPath;
    string inventoryPath;
    string questNpcPath;
    string guidePath;

    public PlayerData playerData;
    public Inventory inventory;
    public NpcQuest npcQuest;
    public Guide guide;

    public List<QuestData> npcQuestList;

    public Dictionary<int, FishData> fishDataDict;
    public Dictionary<int, RodData> rodDataDict;
    public Dictionary<int, ReelData> reelDataDict;
    public Dictionary<int, WireData> wireDataDict;
    public Dictionary<int, HookData> hookDataDict;
    public Dictionary<int, BaitData> baitDataDict;
    public Dictionary<int, ItemData> itemDataDict;
    public Dictionary<int, QuestData> questDataDict;

    void Awake()
    {
        Instance = this;
        Init();
    }

    private void Init() {
        playerPath = Path.Combine(Application.persistentDataPath, "playerdata.json");
        inventoryPath = Path.Combine(Application.persistentDataPath, "inventory.json");
        questNpcPath = Path.Combine(Application.persistentDataPath, "questNpc.json");
        guidePath = Path.Combine(Application.persistentDataPath, "guide.json");

        LoadFishDataFromSo();
        LoadRodDataFromSo();
        LoadReelDataFromSo();
        LoadWireDataFromSo();
        LoadHookDataFromSo();
        LoadBaitDataFromSo();
        LoadItemDataFromSo();
        LoadQuestDataFromSo();

        if(!File.Exists(playerPath)) {
            playerData = new();
            playerData.pos = new Vector3(1275.5f, -75.2f, 1921.3f);
            playerData.rodList.Add(0);
            SetPref();
            SavePlayerData();
        }
        else {
            LoadPlayerData();
        }
        
        if(!File.Exists(inventoryPath)) {
            inventory = new();
            SaveInventoryData();
            LoadInventoryData();
        }
        else {
            LoadInventoryData();
        }

        if(!File.Exists(questNpcPath)) {
            npcQuest = new();
            SetBaseQuest();
            SaveQuestNpcData();
        }
        else {
            LoadQuestNpcData();
        }

        if(!File.Exists(guidePath)) {
            guide = new(fishDataDict.Count);
            SaveGuideData();
        }
        else {
            LoadGuideData();
        }        
    }

    private void SetPref() {
        PlayerPrefs.SetFloat("Master", 0.5f);
        PlayerPrefs.SetFloat("BGM", 0.5f);
        PlayerPrefs.SetFloat("Ambient", 0.5f);
        PlayerPrefs.SetFloat("SFX", 0.5f);
        PlayerPrefs.SetFloat("Mouse", 1f);
    }

    public void SetBaseQuest() {
        npcQuestList.Add(GetQuestData(0));
    }

    // 데이터 불러오기
    private void LoadFishDataFromSo() {
        FishData[] fishDataArray = Resources.LoadAll<FishData>("FishData");
        fishDataDict = new Dictionary<int, FishData>();
        foreach(FishData fish in fishDataArray) {
            fishDataDict[fish.fishID] = fish;
        }
    }

    private void LoadRodDataFromSo() {
        RodData[] rodDataArray = Resources.LoadAll<RodData>("RodData");
        rodDataDict = new Dictionary<int, RodData>();
        foreach(RodData rod in rodDataArray) {
            rodDataDict[rod.rodID] = rod;
        }
    }

    private void LoadReelDataFromSo() {
        ReelData[] reelDataArray = Resources.LoadAll<ReelData>("ReelData");
        reelDataDict = new Dictionary<int, ReelData>();
        foreach(ReelData reel in reelDataArray) {
            reelDataDict[reel.reelID] = reel;
        }
    }

    private void LoadWireDataFromSo() {
        WireData[] wireDataArray = Resources.LoadAll<WireData>("WireData");
        wireDataDict = new Dictionary<int, WireData>();
        foreach(WireData wire in wireDataArray) {
            wireDataDict[wire.wireID] = wire;
        }
    }

    private void LoadHookDataFromSo() {
        HookData[] hookDataArray = Resources.LoadAll<HookData>("HookData");
        hookDataDict = new Dictionary<int, HookData>();
        foreach(HookData hook in hookDataArray) {
            hookDataDict[hook.hookID] = hook;
        }
    }

    private void LoadBaitDataFromSo() {
        BaitData[] baitDataArray = Resources.LoadAll<BaitData>("BaitData");
        baitDataDict = new Dictionary<int, BaitData>();
        foreach (BaitData bait in baitDataArray) {
            baitDataDict[bait.baitID] = bait;
        }
    }
    
    private void LoadItemDataFromSo() {
        ItemData[] itemDataArray = Resources.LoadAll<ItemData>("ItemData");
        itemDataDict = new Dictionary<int, ItemData>();
        foreach(ItemData item in itemDataArray) {
            itemDataDict[item.itemID] = item;
        }
    }

    private void LoadQuestDataFromSo() {
        QuestData[] questDataArray = Resources.LoadAll<QuestData>("QuestData");
        questDataDict = new Dictionary<int, QuestData>();
        foreach(QuestData quest in questDataArray) {
            questDataDict[quest.questID] = quest;
        }
    }

    public void SavePlayerData() {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerPath, json);
    }

    public void LoadPlayerData() {
        string json = File.ReadAllText(playerPath);
        playerData = JsonUtility.FromJson<PlayerData>(json);
    }

    public void SaveInventoryData() {
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(inventoryPath, json);
    }

    public void LoadInventoryData() {
        string json = File.ReadAllText(inventoryPath);
        inventory = JsonUtility.FromJson<Inventory>(json);
    }

    public void SaveQuestNpcData() {
        npcQuest.questList.Clear();
        foreach(var item in npcQuestList) {
            npcQuest.questList.Add(item.questID);
        }
        string json = JsonUtility.ToJson(npcQuest, true);
        File.WriteAllText(questNpcPath, json);
    }

    public void LoadQuestNpcData() {
        string json = File.ReadAllText(questNpcPath);
        npcQuest = JsonUtility.FromJson<NpcQuest>(json);
        foreach(var item in npcQuest.questList) {
            npcQuestList.Add(GetQuestData(item));
        }
    }

    public void SaveGuideData() {
        string json = JsonUtility.ToJson(guide, true);
        File.WriteAllText(guidePath, json);
    }

    public void LoadGuideData() {
        string json = File.ReadAllText(guidePath);
        guide = JsonUtility.FromJson<Guide>(json);
    }

    public RodData GetRodData(int id) {
        return rodDataDict.TryGetValue(id, out RodData rod) ? rod : null;
    }

    public ReelData GetReelData(int id) {
        return reelDataDict.TryGetValue(id, out ReelData reel) ? reel : null;
    }

    public WireData GetWireData(int id) {
        return wireDataDict.TryGetValue(id, out WireData wire) ? wire : null;
    }

    public HookData GetHookData(int id) {
        return hookDataDict.TryGetValue(id, out HookData hook) ? hook : null;
    }

    public BaitData GetBaitData(int id) {
        return baitDataDict.TryGetValue(id, out BaitData bait) ? bait : null;
    }

    public FishData GetFishData(int id) {
        return fishDataDict.TryGetValue(id, out FishData fish) ? fish : null;
    }

    public ItemData GetItemData(int id) {
        return itemDataDict.TryGetValue(id, out ItemData item) ? item : null;
    }

    public QuestData GetQuestData(int id) {
        return questDataDict.TryGetValue(id, out QuestData quest) ? quest : null;
    }

    public void SaveAndExit() {

    }
}

[System.Serializable]
public class PlayerData {
    // 스테미나, 인벤토리
    public int gold = 0;
    public List<int> completeQuest = new();
    public Vector3 pos;
    public bool[] farmUnlock = new bool[4];
    public int donateCount = 0;
    public bool getRelicReward = false;
    public bool getStatue = false;
    public List<int> museumComplete = new();
    public int curRod = 0;
    public List<int> rodList = new();
}

[System.Serializable]
public class NpcQuest {
    public List<int> questList = new();
    public List<NormalQuest> normalQuests = new();
    public int timer;
}

[Serializable]
public class NormalQuest {
    public int receive;
    public int complete;
    public QuestRequirement[] questRequirements;
    public int rewardGold;
}

[System.Serializable]
public class Inventory {
    public List<PlayerFish> fishList = new(new PlayerFish[36]);
    public int[] equip = new int[5];
    public List<PlayerFish> fishInFarm = new(new PlayerFish[24]);
    public NewFish[] newFishList = new NewFish[4];
    public FishFarmTimer[] fishFarmTimer = new FishFarmTimer[12];
}

[System.Serializable]
public class PlayerFish {
    public int fishID = -1;
    public int grade;
    public float weight;
    public int price;

    public PlayerFish Clone() {
        PlayerFish playerFish = new PlayerFish();
        playerFish.fishID = this.fishID;
        playerFish.grade = this.grade;
        playerFish.weight = this.weight;
        playerFish.price = this.price;

        return playerFish;
    }
}

[System.Serializable]
public class NewFish {
    public List<PlayerFish> list = new(new PlayerFish[24]);
}

[Serializable]
public class FishFarmTimer {
    public float timer;
    public bool isFullFarm;
} 

[System.Serializable]
public class Guide {
    public List<bool> fishID;
    public List<CatchGrade> fishGrade; 

    public Guide(int size) {
        fishID = new(new bool[size]);
        fishGrade = new(new CatchGrade[size]);

        for(int i = 0; i < fishID.Count; i++) {
            fishID[i] = false;
        }
    }
}

[System.Serializable]
public class CatchGrade {
    public bool[] grade = new bool[4];
}