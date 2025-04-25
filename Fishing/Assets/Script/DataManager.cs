using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

        LoadDatas();

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
            SaveQuestNpcData();
        }
        else {
            LoadQuestNpcData();
        }

        if(!File.Exists(guidePath)) {
            guide = new(40);
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
    
    private void LoadDatas() {
        LoadFishDataFromAddressables();
        LoadRodDataFromSo();
        LoadReelDataFromSo();
        LoadWireDataFromSo();
        LoadHookDataFromSo();
        LoadBaitDataFromSo();
        LoadItemDataFromSo();
        LoadQuestDataFromSo();
    }

    // 데이터 불러오기
    private async void LoadFishDataFromAddressables() {
        fishDataDict = new Dictionary<int, FishData>();
        var handle = Addressables.LoadAssetsAsync<FishData>("FishData", fish => {
            fishDataDict[fish.fishID] = fish;
        });

        await handle.Task;
    }

    public async Task WaitForFishData() {
        while (fishDataDict == null || fishDataDict.Count == 0) {
            await Task.Yield();
        }
    }

    private async void LoadRodDataFromSo() {
        rodDataDict = new Dictionary<int, RodData>();
        var handle = Addressables.LoadAssetsAsync<RodData>("RodData", rod => {
            rodDataDict[rod.rodID] = rod;
        });

        await handle.Task;
    }

    public async Task WaitForRodData() {
        while (rodDataDict == null || rodDataDict.Count == 0) {
            await Task.Yield();
        }
    }

    private async void LoadReelDataFromSo() {
        reelDataDict = new Dictionary<int, ReelData>();
        var handle = Addressables.LoadAssetsAsync<ReelData>("ReelData", reel => {
            reelDataDict[reel.reelID] = reel;
        });

        await handle.Task;
    }

    private async void LoadWireDataFromSo() {
        wireDataDict = new Dictionary<int, WireData>();
        var handle = Addressables.LoadAssetsAsync<WireData>("WireData", wire => {
            wireDataDict[wire.wireID] = wire;
        });

        await handle.Task;
    }

    private async void LoadHookDataFromSo() {
        hookDataDict = new Dictionary<int, HookData>();
        var handle = Addressables.LoadAssetsAsync<HookData>("HookData", hook => {
            hookDataDict[hook.hookID] = hook;
        });

        await handle.Task;
    }

    private async void LoadBaitDataFromSo() {
        baitDataDict = new Dictionary<int, BaitData>();
        var handle = Addressables.LoadAssetsAsync<BaitData>("BaitData", bait => {
            baitDataDict[bait.baitID] = bait;
        });

        await handle.Task;
    }
    
    private async void LoadItemDataFromSo() {
        itemDataDict = new Dictionary<int, ItemData>();
        var handle = Addressables.LoadAssetsAsync<ItemData>("ItemData", item => {
            itemDataDict[item.itemID] = item;
        });

        await handle.Task;
    }

    private async void LoadQuestDataFromSo() {
        questDataDict = new Dictionary<int, QuestData>();
        var handle = Addressables.LoadAssetsAsync<QuestData>("QuestData", quest => {
            questDataDict[quest.questID] = quest;
        });

        await handle.Task;
    }

    public async Task WaitForQuestData() {
        while (questDataDict == null || questDataDict.Count == 0) {
            await Task.Yield();
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
        string json = JsonUtility.ToJson(npcQuest, true);
        File.WriteAllText(questNpcPath, json);
    }

    public void LoadQuestNpcData() {
        string json = File.ReadAllText(questNpcPath);
        npcQuest = JsonUtility.FromJson<NpcQuest>(json);
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

    public NpcQuest() {
        questList.Add(0);
    }
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