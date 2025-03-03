using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, ISlotHandler
{
    //플레이어 물고기 리스트 + 착용 중 장비 + 소지중인 장비
    [SerializeField] GameObject[] slots;
    [SerializeField] Transform[] details;
    [SerializeField] GameObject[] equip;
    [SerializeField] TMP_Text gold;
    [SerializeField] Transform handPos;
    [SerializeField] PlayerInventory playerInventory;

    private List<ItemData> itemList;
    private List<ItemData> equipList;

    void Start()
    {
        
    }

    public void AddFishToSlot(int id) {

    }

    public void AddItemToSlot(int id) {
        ItemData itemData = DataManager.Instance.GetItemData(id);
        for(int i = 0; i < itemList.Count; i++) {
            if(itemList[i] == null) {
                itemList[i] = itemData;
                slots[i].GetComponent<Image>().sprite = itemData.itemImage;
                slots[i].GetComponent<Button>().onClick.AddListener(() => OnItemClick(itemData));
                slots[i].GetComponent<DraggableItem>().canDrag = true;
                Debug.Log("아이템 이벤토리에 추가");
                DataManager.Instance.SaveInventoryData();
                break;
            }
        }
    }

    private void OnItemClick(ItemData itemData) {
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

    public void SetReelDetail(int id) {
        details[1].gameObject.SetActive(true);
    }

    public void SetWireDetail(int id) {
        details[2].gameObject.SetActive(true);
    }

    public void SetHookDetail(int id) {
        details[3].gameObject.SetActive(true);
    }

    public void SetBaitDetail(int id) {
        details[4].gameObject.SetActive(true);
    }

    public void SwapItem(int indexA, int indexB) {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = slots[indexB].GetComponent<Button>();
        if(itemList[indexB] == null) {
            itemList[indexB] = DataManager.Instance.GetItemData(itemList[indexA].itemID);
            itemList[indexA] = null;

            slots[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = null;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonB.onClick.AddListener(() => OnItemClick(itemList[indexB]));

            slots[indexA].GetComponent<DraggableItem>().canDrag = false;
            slots[indexB].GetComponent<DraggableItem>().canDrag = true;
        }
        else {
            ItemData temp = DataManager.Instance.GetItemData(itemList[indexA].itemID);;
            itemList[indexA] = DataManager.Instance.GetItemData(itemList[indexB].itemID);;
            itemList[indexB] = temp;

            Sprite tempSprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = slots[indexB].GetComponent<Image>().sprite;
            slots[indexB].GetComponent<Image>().sprite = tempSprite;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA]));
            buttonB.onClick.AddListener(() => OnItemClick(itemList[indexB]));
        }
        DataManager.Instance.SaveInventoryData();
    }

    public void ExchangeEquip(int indexA, int indexB) {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = equip[indexB].GetComponent<Button>();
        if(equipList[indexB] == null) {
            equipList[indexB] = DataManager.Instance.GetItemData(itemList[indexA].itemID);;
            itemList[indexA] = null;

            if(indexB == 0) {
                RodData rodData = DataManager.Instance.GetRodData(equipList[indexB].itemID);
                Instantiate(rodData.rodPrefab, handPos);
                playerInventory.SetEquip();
            }

            equip[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = null;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.AddListener(() => OnItemClick(equipList[indexB]));
            
            slots[indexA].GetComponent<DraggableItem>().canDrag = false;
            equip[indexB].GetComponent<DraggableItem>().canDrag = true;
        }
        else if(itemList[indexA] == null) {
            itemList[indexA] = DataManager.Instance.GetItemData(equipList[indexB].itemID);;
            equipList[indexB] = null;

            if(indexB == 0) {
                Destroy(handPos.GetChild(0).gameObject);
            }

            slots[indexA].GetComponent<Image>().sprite = equip[indexB].GetComponent<Image>().sprite;
            equip[indexB].GetComponent<Image>().sprite = null;

            buttonB.onClick.RemoveAllListeners();
            buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA]));

            slots[indexA].GetComponent<DraggableItem>().canDrag = true;
            equip[indexB].GetComponent<DraggableItem>().canDrag = false;
        }
        else {
            ItemData temp = DataManager.Instance.GetItemData(itemList[indexA].itemID);;
            itemList[indexA] = DataManager.Instance.GetItemData(equipList[indexB].itemID);;
            equipList[indexB] = temp;

            if(indexB == 0) {
                Destroy(handPos.GetChild(0).gameObject);
                RodData rodData = DataManager.Instance.GetRodData(equipList[indexB].itemID);
                Instantiate(rodData.rodPrefab, handPos);
                playerInventory.SetEquip();
            }

            Sprite tempSprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = equip[indexB].GetComponent<Image>().sprite;
            equip[indexB].GetComponent<Image>().sprite = tempSprite;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA]));
            buttonB.onClick.AddListener(() => OnItemClick(equipList[indexB]));
        }
        playerInventory.SetRodPower();
        DataManager.Instance.SaveInventoryData();
    }

    public bool CheckType(int indexA, int indexB) {
        if(itemList[indexA] == null) {
            return true;
        }
        switch (indexB) {
            case 0:
                if (itemList[indexA].itemType == ItemType.Rod) {
                    return true;
                }
                break;
            case 1:
                if (itemList[indexA].itemType == ItemType.Reel) {
                    return true;
                }
                break;
            case 2:
                if (itemList[indexA].itemType == ItemType.Wire) {
                    return true;
                }
                break;
            case 3: 
                if (itemList[indexA].itemType == ItemType.Hook) {
                    return true;
                }
                break;
            case 4:
                if (itemList[indexA].itemType == ItemType.Bait) {
                    return true;
                }
                break;
        }
        return false;
    }

    public void SetPlayerSlots() {
        for(int i = 0; i < itemList.Count; i++) {
            int index = i;
            slots[index].GetComponent<Image>().sprite = null;
            slots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            if(itemList[index] != null) {
                slots[index].GetComponent<Image>().sprite = itemList[index].itemImage;
                slots[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(itemList[index]));
                slots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    public void SetPlayerEquip() {
        for(int i = 0; i < equip.Length; i++) {
            int index = i;
            equip[index].GetComponent<Image>().sprite = null;
            equip[index].GetComponent<Button>().onClick.RemoveAllListeners();
            if(equipList[index] != null) {
                equip[index].GetComponent<Image>().sprite = equipList[index].itemImage;
                equip[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(equipList[index]));
                equip[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
        if(equipList[0] != null && handPos.childCount == 0) {
            RodData rodData = DataManager.Instance.GetRodData(equipList[0].itemID);
            Instantiate(rodData.rodPrefab, handPos);
            playerInventory.SetEquip();
            playerInventory.SetRodPower();
        }
    }

    public void SetGoldText() {
        gold.text = DataManager.Instance.playerData.gold + " 코인";
    }

    void OnEnable()
    {
        itemList = DataManager.Instance.inventory.slots;
        equipList = DataManager.Instance.inventory.equip;
        SetPlayerSlots();
        SetPlayerEquip();
        SetGoldText();
    }

    public void DefaultSetting() {
        itemList = DataManager.Instance.inventory.slots;
        equipList = DataManager.Instance.inventory.equip;

        for (int i = 0; i < slots.Length; i++) {
            DraggableItem draggable = slots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;
            
            DropSlot dropSlot = slots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 0;
            dropSlot.inventoryManager = this;
            dropSlot.slotHandler = this;
        }

        for (int i = 0; i < equip.Length; i++) {
            DraggableItem draggable = equip[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 1;

            DropSlot dropSlot = equip[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 1;
            dropSlot.inventoryManager = this;
            dropSlot.slotHandler = this;
        }
        SetPlayerSlots();
        SetPlayerEquip();
        SetGoldText();
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
