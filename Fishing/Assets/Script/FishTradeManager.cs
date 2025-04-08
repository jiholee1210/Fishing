using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishTradeManager : MonoBehaviour, ISlotHandler
{
    [SerializeField] GameObject[] slots;
    [SerializeField] TMP_Text gold;
    [SerializeField] Transform detail;
    [SerializeField] private Button sellButton;

    private List<PlayerFish> fishList;
    private PlayerData playerData;
    private Sprite[] gradeSprites;

    private int curIndex = -1;

    void Start()
    {
        fishList = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        gradeSprites = DataManager.Instance.gradeSprites;

        for (int i = 0; i < slots.Length; i++) {
            DraggableItem draggable = slots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;
            
            DropSlot dropSlot = slots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 0;
            dropSlot.slotHandler = this;
        }

        sellButton.onClick.AddListener(() => SellFish());
    }

    private void SetDetail(PlayerFish fishData, int index) {
        detail.gameObject.SetActive(true);
        curIndex = index;
        FishData fish = DataManager.Instance.GetFishData(fishData.fishID);
        TMP_Text name = detail.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = detail.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text desc = detail.GetChild(3).GetComponent<TMP_Text>();
        TMP_Text weight = detail.GetChild(4).GetComponent<TMP_Text>();
        TMP_Text price = detail.GetChild(5).GetComponent<TMP_Text>();

        Image image = detail.GetChild(1).GetComponent<Image>();
        image.sprite = fish.fishDetail;
        image.SetNativeSize();

        Image grade = detail.GetChild(6).GetComponent<Image>();
        grade.sprite = gradeSprites[fishData.grade];

        name.text = fish.fishName;
        rarity.text = fish.rarity.ToString();
        desc.text = fish.desc;
        weight.text = fishData.weight + "kg";
        price.text = fishData.price + " 골드";
    }

    public void SwapItem(int indexA, int indexB)
    {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = slots[indexB].GetComponent<Button>();
        if(fishList[indexB].fishID == -1) {
            fishList[indexB] = fishList[indexA].Clone();
            fishList[indexA] = null;

            slots[indexB].GetComponent<Image>().sprite = slots[indexA].GetComponent<Image>().sprite;
            slots[indexB].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            slots[indexA].GetComponent<Image>().sprite = null;
            slots[indexA].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            slots[indexB].transform.GetChild(0).GetComponent<Image>().sprite = slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite;
            slots[indexB].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[indexA].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonB.onClick.AddListener(() => SetDetail(fishList[indexB], indexB));

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

            Sprite tempGrade = slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite;
            slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite = slots[indexB].transform.GetChild(0).GetComponent<Image>().sprite;
            slots[indexB].transform.GetChild(0).GetComponent<Image>().sprite = tempGrade;

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => SetDetail(fishList[indexA], indexA));
            buttonB.onClick.AddListener(() => SetDetail(fishList[indexB], indexB));
        }
        DataManager.Instance.SaveInventoryData();
    }

    public void SellFish() {
        if(fishList[curIndex].fishID != -1 && curIndex != -1) {
            playerData.gold += fishList[curIndex].price;
            slots[curIndex].GetComponent<Image>().sprite = null;
            slots[curIndex].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[curIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            slots[curIndex].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            slots[curIndex].GetComponent<Button>().onClick.RemoveAllListeners();
            fishList[curIndex] = null;
            SetGoldText();
            detail.gameObject.SetActive(false);
            DataManager.Instance.SavePlayerData();
            DataManager.Instance.SaveInventoryData();
            curIndex = -1;
        }    
    }

    private void SetSlots() {
        for(int i = 0; i < slots.Length; i++) {
            int index = i;
            slots[index].GetComponent<Image>().sprite = null;
            slots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            slots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            slots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            if(fishList[index].fishID != -1) {
                slots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishList[index].fishID).fishIcon;
                slots[index].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishList[index].grade];
                slots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                slots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                slots[index].GetComponent<Button>().onClick.AddListener(() => SetDetail(fishList[index], index));
                slots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    public void SetGoldText() {
        gold.text = playerData.gold + " 코인";
    }

    public void DefaultSetting() {
        SetSlots();
        SetGoldText();
    }

    public void CloseWindow() {
        detail.gameObject.SetActive(false);
        curIndex = -1;
    }
}
