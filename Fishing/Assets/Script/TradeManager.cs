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
                npcItemList = new(new ItemData[npcListSize]);
                List<ItemData> items = npcObject.GetComponent<IMerchant>().GetItemList(index);
                for(int j = 0; j < items.Count; j++) {
                    npcItemList[j] = items[j];
                }
                SetNpcSlots(npcItemList);

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

    private void SetNpcSlots(List<ItemData> item) {
        for(int i = 0; i < item.Count; i++) {
            int index = i;
            npcSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            npcSlots[index].GetComponent<Image>().sprite = null;
            if(item[index] != null) {
                npcSlots[index].GetComponent<Image>().sprite = item[index].itemImage;
                npcSlots[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(item[index], index));
                npcSlots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    public void SetNpcDefault() {
        for(int i = 0; i < npcItemList.Count; i++) {
            int index = i;
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
        Debug.Log("슬롯 번호 " + i);
        switch(itemData.itemType) {
            case ItemType.Rod:
                SetRodDetail(itemData.itemID, i);
                break;
            case ItemType.Reel:
                SetReelDetail(itemData.itemID, i);
                break;
            case ItemType.Wire:
                SetWireDetail(itemData.itemID, i);
                break;
            case ItemType.Hook:
                SetHookDetail(itemData.itemID, i);
                break;
            case ItemType.Bait:
                SetBaitDetail(itemData.itemID, i);
                break;
        }
    }

    public void SetRodDetail(int itemID, int index) {
            if(details[0].GetChild(5).childCount > 0) {
                    Destroy(details[0].GetChild(5).GetChild(0).gameObject);    
            }

            TMP_Text name = details[0].GetChild(0).GetComponent<TMP_Text>();
            TMP_Text rarity = details[0].GetChild(2).GetComponent<TMP_Text>();
            TMP_Text power = details[0].GetChild(3).GetComponent<TMP_Text>();
            TMP_Text desc = details[0].GetChild(4).GetComponent<TMP_Text>();
            TMP_Text gold = details[0].GetChild(6).GetComponent<TMP_Text>();

            RodData rodData = DataManager.Instance.GetRodData(itemID);

            name.text = rodData.rodName;
            rarity.text = rodData.rodRarity;
            power.text = rodData.rodDur + " 내구력";
            desc.text = rodData.rodDesc;
            gold.text = npcItemList[index].reqGold + " 골드";

            Instantiate(rodData.rodPrefab, details[0].GetChild(5));
            Debug.Log("자식 생성");
            details[0].gameObject.SetActive(true);
            details[0].GetChild(5).GetComponent<RotateRod>().StartRotate();
    }

    public void SetReelDetail(int id, int index) {
        details[1].gameObject.SetActive(true);
    }

    public void SetWireDetail(int id, int index) {
        details[2].gameObject.SetActive(true);
    }

    public void SetHookDetail(int id, int index) {
        details[3].gameObject.SetActive(true);
    }

    public void SetBaitDetail(int id, int index) {
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

    public void CloseWindow() {
        if(details[0].GetChild(5).childCount > 0) {
            Destroy(details[0].GetChild(5).GetChild(0).gameObject);
        }

        foreach(Transform detail in details) {
            detail.gameObject.SetActive(false);
        }
    }
}
