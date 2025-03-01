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
    [SerializeField] Transform handPos;
    [SerializeField] PlayerInventory playerInventory;

    private List<ItemData> itemList;
    private List<ItemData> equipList;
    void Start()
    {
        int equipSize = 5;
        equipList = new List<ItemData>(new ItemData[equipSize]);

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
    }

    public void AddFishToSlot(int id) {

    }

    public void AddItemToSlot(int id) {
        ItemData itemData = DataManager.Instance.GetItemData(id);
        for(int i = 0; i < itemList.Count; i++) {
            if(itemList[i] == null) {
                itemList[i] = itemData;
                slots[i].GetComponent<Image>().sprite = itemData.itemImage;
                slots[i].GetComponent<Button>().onClick.AddListener(() => OnItemClick(itemData, i));
                Debug.Log("아이템 이벤토리에 추가");
                playerInventory.SetSlots(itemList);
                break;
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

    public void SwapItem(int indexA, int indexB) {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = slots[indexB].GetComponent<Button>();
        if(itemList[indexB] == null) {
            itemList[indexB] = itemList[indexA].Clone();
            itemList[indexA] = null;

            slots[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = null;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonB.onClick.AddListener(() => OnItemClick(itemList[indexB], indexB));
            return;
        }
        ItemData temp = itemList[indexA].Clone();
        itemList[indexA] = itemList[indexB].Clone();
        itemList[indexB] = temp;

        Sprite tempSprite = slots[indexA].GetComponent<Image>().sprite;
        slots[indexA].GetComponent<Image>().sprite = slots[indexB].GetComponent<Image>().sprite;
        slots[indexB].GetComponent<Image>().sprite = tempSprite;

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();

        buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA], indexA));
        buttonB.onClick.AddListener(() => OnItemClick(itemList[indexB], indexB));

    }

    public void ExchangeEquip(int indexA, int indexB) {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = equip[indexB].GetComponent<Button>();
        if(equipList[indexB] == null) {
            equipList[indexB] = itemList[indexA].Clone();
            itemList[indexA] = null;

            if(indexB == 0) {
                RodData rodData = DataManager.Instance.GetRodData(equipList[indexB].itemID);
                Instantiate(rodData.rodPrefab, handPos);
            }

            equip[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = null;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.AddListener(() => OnItemClick(equipList[indexB], indexB));
            
        }
        else if(itemList[indexA] == null) {
            itemList[indexA] = equipList[indexB].Clone();
            equipList[indexB] = null;

            if(indexB == 0) {
                Destroy(handPos.GetChild(0).gameObject);
            }

            slots[indexA].GetComponent<Image>().sprite = equip[indexB].GetComponent<Image>().sprite;
            equip[indexB].GetComponent<Image>().sprite = null;

            buttonB.onClick.RemoveAllListeners();
            buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA], indexA));

        }
        else {
            ItemData temp = itemList[indexA].Clone();
            itemList[indexA] = equipList[indexB].Clone();
            equipList[indexB] = temp;

            if(indexB == 0) {
                Destroy(handPos.GetChild(0).gameObject);
                RodData rodData = DataManager.Instance.GetRodData(equipList[indexB].itemID);
                Instantiate(rodData.rodPrefab, handPos);
            }

            Sprite tempSprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = equip[indexB].GetComponent<Image>().sprite;
            equip[indexB].GetComponent<Image>().sprite = tempSprite;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => OnItemClick(itemList[indexA], indexA));
            buttonB.onClick.AddListener(() => OnItemClick(equipList[indexB], indexB));
        }

        playerInventory.SetSlots(itemList);
        playerInventory.SetEquip(equipList);
        playerInventory.SetRodPower();
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
        itemList = DataManager.Instance.inventory.slots;

        for(int i = 0; i < itemList.Count; i++) {
            int index = i;
            if(itemList[index] != null) {
                slots[index].GetComponent<Image>().sprite = itemList[index].itemImage;
                slots[index].GetComponent<Button>().onClick.AddListener(() => OnItemClick(itemList[index], index));
                Debug.Log(index + " " + itemList[index].itemID);
            }
            else {
                slots[index].GetComponent<Image>().sprite = null;
                slots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
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
