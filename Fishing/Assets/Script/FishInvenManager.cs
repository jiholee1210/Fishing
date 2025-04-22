using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class FishInvenManager : MonoBehaviour, ISlotHandler
{
    [SerializeField] GameObject[] slots;
    [SerializeField] GameObject[] equips;
    [SerializeField] Transform detail;
    [SerializeField] TMP_Text goldText;
    [SerializeField] Sprite[] gradeSprite;
    [SerializeField] TooltipManager tooltipManager;

    private List<PlayerFish> fishList;
    private int[] equipList = new int[5];
    private PlayerData playerData;

    private Color[] rarityColor = {new Color(0f, 0f, 0f), new Color(0f, 0.6f, 0.9f), new Color(0.7f, 0f, 1f), new Color(1f, 0.3f, 0.1f), new Color(0f, 0.8f, 0.6f)};

    void Start()
    {
        fishList = DataManager.Instance.inventory.fishList;
        equipList = DataManager.Instance.inventory.equip;
        playerData = DataManager.Instance.playerData;

        for (int i = 0; i < slots.Length; i++) {
            DraggableItem draggable = slots[i].AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            draggable.slotType = 0;
            
            DropSlot dropSlot = slots[i].AddComponent<DropSlot>();
            dropSlot.slotIndex = i;
            dropSlot.slotType = 0;
            dropSlot.slotHandler = this;
        }
    }

    private void SetDetail(PlayerFish fishData) {
        SoundManager.Instance.ButtonClick();
        detail.gameObject.SetActive(true);

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
        grade.sprite = gradeSprite[fishData.grade];

        name.text = fish.fishName;
        rarity.text = fish.rarityLocalized;
        rarity.color = rarityColor[(int)fish.rarity];
        desc.text = fish.desc;
        weight.text = fishData.weight + "kg";
        price.text = fishData.price + " " + LocalizationSettings.StringDatabase.GetLocalizedString("DialogTable", "coin");
    }
    
    public void SwapItem(int indexA, int indexB)
    {
        Button buttonA = slots[indexA].GetComponent<Button>();
        Button buttonB = slots[indexB].GetComponent<Button>();
        if(fishList[indexB].fishID == -1) {
            Debug.Log("빈칸에 스왑");
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

            Sprite tempGrade = slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite;
            slots[indexA].transform.GetChild(0).GetComponent<Image>().sprite = slots[indexB].transform.GetChild(0).GetComponent<Image>().sprite;
            slots[indexB].transform.GetChild(0).GetComponent<Image>().sprite = tempGrade;


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
            slots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            slots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            slots[index].GetComponent<Button>().onClick.RemoveAllListeners();

            if(fishList[index].fishID != -1) {
                slots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishList[i].fishID).fishIcon;
                slots[index].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishList[index].grade];
                slots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                slots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                slots[index].GetComponent<Button>().onClick.AddListener(() => SetDetail(fishList[index]));
                slots[index].GetComponent<DraggableItem>().canDrag = true;
            }
        }
    }

    private void SetGoldText() {
        goldText.text = playerData.gold + " C";
    }

    private void SetEquipSlots() {
        for (int i = 0; i < equipList.Length; i++) {
            int index = i;
            int itemID = index * 10 + equipList[index];
            equips[index].GetComponent<Image>().sprite = DataManager.Instance.GetItemData(itemID).itemImage;
            equips[index].GetComponent<Slot>().itemID = itemID;
        }
    }

    public void DefaultSetting() {
        SetSlots();
        SetEquipSlots();
        SetGoldText();
    }

    public void CloseWindow() {
        detail.gameObject.SetActive(false);
        tooltipManager.HideTooltip();
    }
}
