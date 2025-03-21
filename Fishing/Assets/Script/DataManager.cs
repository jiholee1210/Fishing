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
    string questNpcPath;
    string guidePath;

    public PlayerData playerData;
    public Inventory inventory;
    public NpcQuest npcQuest;
    public Guide guide;

    public List<ItemData> slotList;
    public List<ItemData> equipList;
    public List<QuestData> playerQuest;
    public List<QuestData> tutorial;
    public List<QuestData> island;
    public List<QuestData> rock;
    public List<QuestData> lava;

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
            SavePlayerData();
            Debug.Log("데이터 새로 생성");
        }
        else {
            LoadPlayerData();
        }
        
        if(!File.Exists(inventoryPath)) {
            inventory = new();
            SaveInventoryData();
            LoadInventoryData();
            Debug.Log("인벤토리 생성");
        }
        else {
            LoadInventoryData();
        }

        if(!File.Exists(questNpcPath)) {
            npcQuest = new();
            SetBaseQuest();
            SaveQuestNpcData();
            Debug.Log("퀘스트 상황 생성");
        }
        else {
            LoadQuestNpcData();
        }

        if(!File.Exists(guidePath)) {
            guide = new();
            SaveGuideData();
            Debug.Log("도감 생성");
        }
        else {
            LoadGuideData();
        }        
    }

    public void SetBaseQuest() {
        tutorial.Add(GetQuestData(0));
        island.Add(GetQuestData(1));
    }

    // 데이터 불러오기
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

    private void LoadReelDataFromSo() {
        ReelData[] reelDataArray = Resources.LoadAll<ReelData>("ReelData");
        reelDataDict = new Dictionary<int, ReelData>();
        foreach(ReelData reel in reelDataArray) {
            reelDataDict[reel.reelID] = reel;
        }
        Debug.Log("낚시 릴 데이터 불러오기");
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
        Debug.Log("아이템 데이터 불러오기");
    }

    private void LoadQuestDataFromSo() {
        QuestData[] questDataArray = Resources.LoadAll<QuestData>("QuestData");
        questDataDict = new Dictionary<int, QuestData>();
        foreach(QuestData quest in questDataArray) {
            questDataDict[quest.questID] = quest;
        }
    }

    public void SavePlayerData() {
        List<int> quest = new();
        foreach(var item in playerQuest) {
            quest.Add(item.questID);
        }
        playerData.questList = quest;
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerPath, json);
        Debug.Log("데이터 저장");
    }

    public void LoadPlayerData() {
        string json = File.ReadAllText(playerPath);
        playerData = JsonUtility.FromJson<PlayerData>(json);
        foreach(var item in playerData.questList) {
            playerQuest.Add(GetQuestData(item));
        }
        Debug.Log("데이터 로드");
    }

    public void SaveInventoryData() {
        for(int i = 0; i < slotList.Count; i++) {
            inventory.slots[i] = slotList[i] != null ? slotList[i].itemID : -1;
        }
        for(int i = 0; i < equipList.Count; i++) {
            inventory.equip[i] = equipList[i] != null ? equipList[i].itemID : -1;
        }
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(inventoryPath, json);
        Debug.Log("인벤토리 저장");
    }

    public void LoadInventoryData() {
        string json = File.ReadAllText(inventoryPath);
        inventory = JsonUtility.FromJson<Inventory>(json);
        foreach(var item in inventory.slots) {
            slotList.Add(GetItemData(item));
        }
        foreach(var item in inventory.equip) {
            equipList.Add(GetItemData(item));
        }
        Debug.Log("인벤토리 로드");
    }

    public void SaveQuestNpcData() {
        NpcQuest tmp = new();
        foreach(var item in tutorial) {
            tmp.tutorial.Add(item.questID);
        }
        foreach(var item in island) {
            tmp.island.Add(item.questID);
        }
        foreach(var item in rock) {
            tmp.rock.Add(item.questID);
        }
        foreach(var item in lava) {
            tmp.lava.Add(item.questID);
        }
        npcQuest = tmp;
        string json = JsonUtility.ToJson(npcQuest, true);
        File.WriteAllText(questNpcPath, json);
        Debug.Log("퀘스트 진행상황 저장");
    }

    public void LoadQuestNpcData() {
        string json = File.ReadAllText(questNpcPath);
        npcQuest = JsonUtility.FromJson<NpcQuest>(json);
        foreach(var item in npcQuest.tutorial) {
            tutorial.Add(GetQuestData(item));
        }
        foreach(var item in npcQuest.island) {
            island.Add(GetQuestData(item));
        }
        foreach(var item in npcQuest.rock) {
            rock.Add(GetQuestData(item));
        }
        foreach(var item in npcQuest.lava) {
            lava.Add(GetQuestData(item));
        }
        Debug.Log("퀘스트 진행상황 로드");
    }

    public void SaveGuideData() {
        string json = JsonUtility.ToJson(guide, true);
        File.WriteAllText(guidePath, json);
        Debug.Log("도감 저장");
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
    public List<int> questList = new();
}

[System.Serializable]
public class NpcQuest {
    public List<int> tutorial = new();
    public List<int> island = new();
    public List<int> rock = new();
    public List<int> lava = new();
}

[System.Serializable]
public class Inventory {
    public List<PlayerFish> fishList = new(new PlayerFish[36]);
    public List<int> slots;
    public List<int> equip;

    public Inventory() {
        slots = new List<int>(new int[36]);
        equip = new List<int>(new int[5]);

        for (int i = 0; i < slots.Count; i++) {
            slots[i] = -1;
        }
        for (int i = 0; i < equip.Count; i++) {
            equip[i] = -1;
        }
    }
}

[System.Serializable]
public class PlayerFish {
    public int fishID;
    public float weight;
    public int price;

    public PlayerFish Clone() {
        PlayerFish playerFish = new PlayerFish();
        playerFish.fishID = this.fishID;
        playerFish.weight = this.weight;
        playerFish.price = this.price;

        return playerFish;
    }
}

[System.Serializable]
public class Guide {
    public List<bool> fishID;

    public Guide() {
        fishID = new(new bool[22]);

        for(int i = 0; i < fishID.Count; i++) {
            fishID[i] = false;
        }
    }
}