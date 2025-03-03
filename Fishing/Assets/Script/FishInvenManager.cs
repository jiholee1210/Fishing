using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishInvenManager : MonoBehaviour, ISlotHandler
{
    [SerializeField] GameObject[] slots;
    [SerializeField] Transform detail;

    private List<PlayerFish> fishList;

    private void SetDetail(PlayerFish fishData) {
        detail.gameObject.SetActive(true);

        FishData fish = DataManager.Instance.GetFishData(fishData.fishID);
        TMP_Text name = detail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = detail.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text desc = detail.GetChild(3).GetComponent<TMP_Text>();
        TMP_Text weight = detail.GetChild(4).GetComponent<TMP_Text>();
        TMP_Text price = detail.GetChild(5).GetComponent<TMP_Text>();

        Image image = detail.GetChild(1).GetComponent<Image>();

        name.text = fish.fishName;
        rarity.text = fish.rarity;
        desc.text = fish.desc;
        weight.text = fishData.weight + "kg";
        price.text = fishData.price + " 골드";
        image.sprite = fish.fishIcon;
    }
    
    public void SwapItem(int indexA, int indexB)
    {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = slots[indexB].GetComponent<Button>();
        if(fishList[indexB].fishID == 0) {
            fishList[indexB] = fishList[indexA].Clone();
            fishList[indexA] = null;

            slots[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = null;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonB.onClick.AddListener(() => SetDetail(fishList[indexB]));

            slots[indexB].GetComponent<DraggableItem>().canDrag = true;
            slots[indexA].GetComponent<DraggableItem>().canDrag = false;
        }
        else {
            PlayerFish temp = fishList[indexA].Clone();
            fishList[indexA] = fishList[indexB].Clone();
            fishList[indexB] = temp;

            Sprite tempSprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexA].GetComponent<Image>().sprite = slots[indexB].GetComponent<Image>().sprite;
            slots[indexB].GetComponent<Image>().sprite = tempSprite;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => SetDetail(fishList[indexA]));
            buttonB.onClick.AddListener(() => SetDetail(fishList[indexB]));
        }
        DataManager.Instance.SaveInventoryData();
    }

    private void SetSlots() {
        for(int i = 0; i < slots.Length; i++) {
            int index = i;
            slots[index].GetComponent<Image>().sprite = null;
            slots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            if(fishList[index].fishID != 0) {
                slots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishList[i].fishID).fishIcon;
                slots[index].GetComponent<Button>().onClick.AddListener(() => SetDetail(fishList[index]));
                slots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    void OnEnable()
    {
        fishList = DataManager.Instance.inventory.fishList;
        SetSlots();
    }

    public void DefaultSetting() {
        fishList = DataManager.Instance.inventory.fishList;

        for (int i = 0; i < slots.Length; i++) {
            DraggableItem draggable = slots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;
            
            DropSlot dropSlot = slots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 0;
            dropSlot.slotHandler = this;
        }

        SetSlots();
    }

    void OnDisable()
    {
        detail.gameObject.SetActive(false);
    }

    public void CloseWindow() {
            detail.gameObject.SetActive(false);
    }
}
