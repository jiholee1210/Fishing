using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour, ISlotHandler
{
    [SerializeField] GameObject[] npcSlots;
    [SerializeField] GameObject[] playerSlots;
    [SerializeField] Button[] category;
    [SerializeField] Transform[] details;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<ItemData> playerItemList;
    private List<ItemData> npcItemList;
    private GameObject npcObject;

    void Start()
    {
        for(int i = 0; i < playerSlots.Length; i++) {
            DraggableItem draggable = playerSlots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;

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
        if(playerItemList[indexB] == null) {
            playerItemList[indexB] = npcItemList[indexA].Clone();
            playerSlots[indexB].GetComponent<Image>().sprite = npcSlots[indexA].GetComponent<Image>().sprite;
        }
        else {
            for(int i = 0; i < playerItemList.Count; i++) {
                if(playerItemList[i] == null) {
                    Debug.Log(i);
                    playerItemList[i] = npcItemList[indexA].Clone();
                    playerSlots[i].GetComponent<Image>().sprite = npcSlots[indexA].GetComponent<Image>().sprite;
                    return;
                }
            }
        }
    }

    public void SwapItem(int indexA, int indexB) {
        if(playerItemList[indexB] == null) {
            playerItemList[indexB] = playerItemList[indexA].Clone();
            playerItemList[indexA] = null;

            playerSlots[indexB].GetComponent<Image>().sprite = playerSlots[indexA].GetComponent<Image>().sprite;
            playerSlots[indexA].GetComponent<Image>().sprite = null;
            return;
        }
        ItemData temp = playerItemList[indexA].Clone();
        playerItemList[indexA] = playerItemList[indexB].Clone();
        playerItemList[indexB] = temp;

        Sprite tempSprite = playerSlots[indexA].GetComponent<Image>().sprite;
        playerSlots[indexA].GetComponent<Image>().sprite = playerSlots[indexB].GetComponent<Image>().sprite;
        playerSlots[indexB].GetComponent<Image>().sprite = tempSprite;
    }

    public void SetNpcObject(GameObject _npcObject) {
        npcObject = _npcObject;
    }

    private void SetNpcSlots(List<ItemData> item) {
        for(int i = 0; i < item.Count; i++) {
            int index = i;
            if(item[index] != null) {
                npcSlots[index].GetComponent<Image>().sprite = item[index].itemImage;
                npcSlots[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(item[index], index));
            }
            else {
                npcSlots[index].GetComponent<Image>().sprite = null;
                npcSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    public void SetPlayerSlots() {
        playerItemList = DataManager.Instance.inventory.slots;

        for(int i = 0; i < playerItemList.Count; i++) {
            if(playerItemList[i] != null) {
                playerSlots[i].GetComponent<Image>().sprite = playerItemList[i].itemImage;
                Debug.Log(i + " " + playerItemList[i].itemID);
            }
            else {
                playerSlots[i].GetComponent<Image>().sprite = null;
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

            TMP_Text Name = details[0].GetChild(0).GetComponent<TMP_Text>();
            TMP_Text Rarity = details[0].GetChild(2).GetComponent<TMP_Text>();
            TMP_Text Power = details[0].GetChild(3).GetComponent<TMP_Text>();
            TMP_Text Desc = details[0].GetChild(4).GetComponent<TMP_Text>();

            RodData rodData = DataManager.Instance.GetRodData(itemID);

            Name.text = rodData.rodName;
            Rarity.text = rodData.rodRarity;
            Power.text = rodData.rodPower + " 낚시력";
            Desc.text = rodData.rodDesc;

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
        SetPlayerSlots();
    }

    void OnDisable()
    {
        if(details[0].GetChild(5).childCount > 0) {
            Destroy(details[0].GetChild(5).GetChild(0).gameObject);
        }

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
