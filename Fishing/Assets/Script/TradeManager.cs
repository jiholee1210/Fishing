using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour, ISlotHandler
{
    [SerializeField] GameObject[] npcSlots;
    [SerializeField] GameObject[] playerSlots;
    [SerializeField] TMP_Text gold;
    [SerializeField] Button[] category;
    [SerializeField] Transform[] details;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<ItemData> playerItemList;
    private PlayerData playerData;

    private List<ItemData> npcItemList;
    private GameObject npcObject;

    void Start()
    {
        for(int i = 0; i < playerSlots.Length; i++) {
            DraggableItem draggable = playerSlots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;
            if(playerItemList[i] != null) {
                draggable.canDrag = true;
            }

            DropSlot dropSlot = playerSlots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 0;
            dropSlot.tradeManager = this;
            dropSlot.slotHandler = this;
        }

        int npcListSize = 21;
        npcItemList = new(new ItemData[npcListSize]);
        for(int i = 0; i < npcSlots.Length; i++) {
            DraggableItem draggable = npcSlots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 2;

            DropSlot dropSlot = npcSlots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 2;
            dropSlot.tradeManager = this;
            dropSlot.slotHandler = this;
        }

        for(int i = 0; i < category.Length; i++) {
            int index = i;
            category[index].onClick.AddListener(() => {
                //npcItemList = new(new ItemData[npcListSize]);
                List<ItemData> items = npcObject.GetComponent<IMerchant>().GetItemList();
                SetNpcDefault();
                int listID = 0;
                for(int j = 0; j < items.Count; j++) {
                    
                    if(items[j].itemType == (ItemType)index) {
                        Debug.Log(listID);
                        npcItemList[listID++] = items[j];
                    }
                }
                SetNpcSlots(index);
                
                if(details[0].GetChild(5).childCount > 0) {
                    Destroy(details[0].GetChild(5).GetChild(0).gameObject);
                }

                foreach(Transform detail in details) {
                    detail.gameObject.SetActive(false);
                }
            });
        }
    }

    public void BuyEquip(int indexA, int indexB) {
        if(playerData.gold >= npcItemList[indexA].reqGold) {
            if(playerItemList[indexB] == null) {
                playerItemList[indexB] = DataManager.Instance.GetItemData(npcItemList[indexA].itemID);
                playerSlots[indexB].GetComponent<Image>().sprite = npcSlots[indexA].GetComponent<Image>().sprite;
                playerSlots[indexB].GetComponent<DraggableItem>().canDrag = true;
            }
            else {
                for(int i = 0; i < playerItemList.Count; i++) {
                    if(playerItemList[i] == null) {
                        Debug.Log(i);
                        playerItemList[i] = DataManager.Instance.GetItemData(npcItemList[indexA].itemID);
                        playerSlots[i].GetComponent<Image>().sprite = npcSlots[indexA].GetComponent<Image>().sprite;
                        playerSlots[i].GetComponent<DraggableItem>().canDrag = true;
                        break;
                    }
                }
            }
            playerData.gold -= npcItemList[indexA].reqGold;
            SetGoldText();
            DataManager.Instance.SaveInventoryData();
            DataManager.Instance.SavePlayerData();
        }
        else {
            Debug.Log("골드가 부족합니다.");
        }
    }

    public void SellEquip(int indexA) {
        Debug.Log(indexA + " " + playerItemList[indexA].reqGold);
        playerData.gold += (int)(playerItemList[indexA].reqGold * 0.2f);

        playerItemList[indexA] = null;
        playerSlots[indexA].GetComponent<Image>().sprite = null;
        playerSlots[indexA].GetComponent<DraggableItem>().canDrag = false;

        SetGoldText();
        DataManager.Instance.SaveInventoryData();
        DataManager.Instance.SavePlayerData();
    }

    public void SwapItem(int indexA, int indexB) {
        if(playerItemList[indexB] == null) {
            playerItemList[indexB] = DataManager.Instance.GetItemData(playerItemList[indexA].itemID);
            playerItemList[indexA] = null;

            playerSlots[indexB].GetComponent<Image>().sprite = playerSlots[indexA].GetComponent<Image>().sprite;
            playerSlots[indexA].GetComponent<Image>().sprite = null;

            playerSlots[indexA].GetComponent<DraggableItem>().canDrag = false;
            playerSlots[indexB].GetComponent<DraggableItem>().canDrag = true;
        }
        else {
            ItemData temp = DataManager.Instance.GetItemData(playerItemList[indexA].itemID);
            playerItemList[indexA] = DataManager.Instance.GetItemData(playerItemList[indexB].itemID);
            playerItemList[indexB] = temp;

            Sprite tempSprite = playerSlots[indexA].GetComponent<Image>().sprite;
            playerSlots[indexA].GetComponent<Image>().sprite = playerSlots[indexB].GetComponent<Image>().sprite;
            playerSlots[indexB].GetComponent<Image>().sprite = tempSprite;
        } 
        DataManager.Instance.SaveInventoryData();
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }

    private void SetNpcSlots(int idx) {
        for(int i = 0; i < npcItemList.Count; i++) {
            int index = i;
            npcSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            npcSlots[index].GetComponent<Image>().sprite = null;
            if(npcItemList[index] != null) {
                npcSlots[index].GetComponent<Image>().sprite = npcItemList[index].itemImage;
                npcSlots[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(npcItemList[index], index));
                npcSlots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    public void SetNpcDefault() {
        for(int i = 0; i < npcItemList.Count; i++) {
            int index = i;
            npcItemList[index] = null;
            npcSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            npcSlots[index].GetComponent<Image>().sprite = null;
            npcSlots[index].GetComponent<DraggableItem>().canDrag = false;
        }
    }

    public void SetPlayerSlots() {
        playerItemList = DataManager.Instance.slotList;

        for(int i = 0; i < playerItemList.Count; i++) {
            playerSlots[i].GetComponent<Image>().sprite = null;
            if(playerItemList[i] != null) {
                playerSlots[i].GetComponent<Image>().sprite = playerItemList[i].itemImage;
                Debug.Log(i + " " + playerItemList[i].itemID);
            }
        }
    }

    private void OnItemClick(ItemData itemData, int i) {
        switch(itemData.itemType) {
            case ItemType.Rod:
                SetRodDetail(itemData.itemID);
                break;
            case ItemType.Reel:
                SetReelDetail(itemData.itemID);
                break;
            case ItemType.Wire:
                SetWireDetail(itemData.itemID);
                break;
            case ItemType.Hook:
                SetHookDetail(itemData.itemID);
                break;
            case ItemType.Bait:
                SetBaitDetail(itemData.itemID);
                break;
        }
    }

    public void SetRodDetail(int itemID) {
        if(details[0].GetChild(5).childCount > 0) {
                Destroy(details[0].GetChild(5).GetChild(0).gameObject);    
        }

        TMP_Text name = details[0].GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = details[0].GetChild(2).GetComponent<TMP_Text>();
        TMP_Text power = details[0].GetChild(3).GetComponent<TMP_Text>();
        TMP_Text desc = details[0].GetChild(4).GetComponent<TMP_Text>();
        TMP_Text gold = details[0].GetChild(6).GetComponent<TMP_Text>();

        RodData rodData = DataManager.Instance.GetRodData(itemID);
        ItemData itemData = DataManager.Instance.GetItemData(itemID);

        name.text = rodData.rodName;
        rarity.text = rodData.rodRarity;
        power.text = rodData.rodDur + " 내구력";
        desc.text = rodData.rodDesc;
        gold.text = itemData.reqGold + " 골드";

        Instantiate(rodData.rodPrefab, details[0].GetChild(5));
        Debug.Log("자식 생성");
        details[0].gameObject.SetActive(true);
        details[0].GetChild(5).GetComponent<RotateRod>().StartRotate();
    }

    public void SetReelDetail(int id) {
        if(details[0].GetChild(5).childCount > 0) {
                Destroy(details[0].GetChild(5).GetChild(0).gameObject);    
        }

        TMP_Text name = details[1].GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = details[1].GetChild(2).GetComponent<TMP_Text>();
        TMP_Text power = details[1].GetChild(3).GetComponent<TMP_Text>();
        TMP_Text desc = details[1].GetChild(4).GetComponent<TMP_Text>();
        TMP_Text gold = details[1].GetChild(5).GetComponent<TMP_Text>();
        Image image = details[1].GetChild(1).GetComponent<Image>();

        ReelData reelData = DataManager.Instance.GetReelData(id);
        ItemData itemData = DataManager.Instance.GetItemData(id);

        name.text = reelData.reelName;
        rarity.text = reelData.reelRarity;
        power.text = reelData.reelSpeed + " 속도";
        desc.text = reelData.reelDesc;
        image.sprite = itemData.itemImage;
        gold.text = itemData.reqGold + " 골드";

        details[1].gameObject.SetActive(true);
    }

    public void SetWireDetail(int id) {
        TMP_Text name = details[2].GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = details[2].GetChild(2).GetComponent<TMP_Text>();
        TMP_Text power = details[2].GetChild(3).GetComponent<TMP_Text>();
        TMP_Text desc = details[2].GetChild(4).GetComponent<TMP_Text>();
        TMP_Text gold = details[2].GetChild(5).GetComponent<TMP_Text>();
        Image image = details[2].GetChild(1).GetComponent<Image>();

        WireData wireData = DataManager.Instance.GetWireData(id);
        ItemData itemData = DataManager.Instance.GetItemData(id);

        name.text = wireData.wireName;
        rarity.text = wireData.wireRarity;
        power.text = wireData.wirePower + " 파워";
        desc.text = wireData.wireDesc;
        image.sprite = itemData.itemImage;
        gold.text = itemData.reqGold + " 골드";

        details[2].gameObject.SetActive(true);
    }

    public void SetHookDetail(int id) {
        TMP_Text name = details[3].GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = details[3].GetChild(2).GetComponent<TMP_Text>();
        TMP_Text power = details[3].GetChild(3).GetComponent<TMP_Text>();
        TMP_Text desc = details[3].GetChild(4).GetComponent<TMP_Text>();
        TMP_Text gold = details[3].GetChild(5).GetComponent<TMP_Text>();
        Image image = details[3].GetChild(1).GetComponent<Image>();

        HookData hookData = DataManager.Instance.GetHookData(id);
        ItemData itemData = DataManager.Instance.GetItemData(id);

        name.text = hookData.hookName;
        rarity.text = hookData.hookRarity;
        power.text = hookData.hookPower + " 파워";
        desc.text = hookData.hookDesc;
        image.sprite = itemData.itemImage;
        gold.text = itemData.reqGold + " 골드";

        details[3].gameObject.SetActive(true);
    }

    public void SetBaitDetail(int id) {
        TMP_Text name = details[4].GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = details[4].GetChild(2).GetComponent<TMP_Text>();
        TMP_Text power = details[4].GetChild(3).GetComponent<TMP_Text>();
        TMP_Text desc = details[4].GetChild(4).GetComponent<TMP_Text>();
        TMP_Text gold = details[4].GetChild(5).GetComponent<TMP_Text>();
        Image image = details[4].GetChild(1).GetComponent<Image>();

        BaitData baitData = DataManager.Instance.GetBaitData(id);
        ItemData itemData = DataManager.Instance.GetItemData(id);

        name.text = baitData.baitName;
        rarity.text = baitData.baitRarity;
        power.text = baitData.baitLevel + " 단계";
        desc.text = baitData.baitDesc;
        image.sprite = itemData.itemImage;
        gold.text = itemData.reqGold + " 골드";

        details[4].gameObject.SetActive(true);
    }

    void OnEnable()
    {
        playerData = DataManager.Instance.playerData;
        SetPlayerSlots();
        SetGoldText(); 
    }

    public void SetGoldText() {
        gold.text = playerData.gold + " 코인";
    }

    void OnDisable()
    {
        if(details[0].GetChild(5).childCount > 0) {
            Destroy(details[0].GetChild(5).GetChild(0).gameObject);
        }

        SetNpcDefault();

        foreach(Transform detail in details) {
            detail.gameObject.SetActive(false);
        }
    }
}
