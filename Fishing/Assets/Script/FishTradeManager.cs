using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishTradeManager : MonoBehaviour
{
    [SerializeField] GameObject[] slots;
    [SerializeField] TMP_Text gold;
    [SerializeField] private TMP_Text totalGoldText;
    [SerializeField] Transform detail;
    [SerializeField] private Button sellButton;
    [SerializeField] private GameObject selectIconPrefab;
    [SerializeField] private Transform selectIconParents;

    private List<PlayerFish> fishList;
    private PlayerData playerData;
    private Sprite[] gradeSprites;
    private List<int> selectedFish = new();
    private List<GameObject> activeIcon = new();

    private Color[] rarityColor = {new Color(0f, 0f, 0f), new Color(0f, 0.6f, 0.9f), new Color(0.7f, 0f, 1f), new Color(1f, 0.3f, 0.1f), new Color(0f, 0.8f, 0.6f)};
    private int totalGold = 0;

    void Start()
    {
        fishList = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        gradeSprites = DataManager.Instance.gradeSprites;

        sellButton.onClick.AddListener(() => SellFish());
    }

    private void SetDetail(PlayerFish fishData, int index, Vector2 pos) {
        if(selectedFish.Contains(index)) {
            int num = selectedFish.IndexOf(index);
            selectedFish.Remove(index);
            Destroy(activeIcon[num]);
            activeIcon.RemoveAt(num);
            totalGold -= fishData.price;
        } 
        else {
            selectedFish.Add(index);
            GameObject icon = Instantiate(selectIconPrefab, selectIconParents);
            icon.transform.position = new Vector2(pos.x + 20f, pos.y + 20f);
            activeIcon.Add(icon);
            totalGold += fishData.price;
        }
        totalGoldText.text = totalGold + " C";

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
        grade.sprite = gradeSprites[fishData.grade];

        name.text = fish.fishName;
        rarity.text = fish.rarity.ToString();
        rarity.color = rarityColor[(int)fish.rarity];
        desc.text = fish.desc;
        weight.text = fishData.weight + "kg";
        price.text = fishData.price + " 코인";
    }

    public void SellFish() {
        if(selectedFish.Count > 0) {
            SoundManager.Instance.SellFish(); 
            foreach(int id in selectedFish) {
                fishList[id] = null;
                slots[id].GetComponent<Image>().sprite = null;
                slots[id].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[id].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                slots[id].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                slots[id].GetComponent<Button>().onClick.RemoveAllListeners();
            }
            playerData.gold += totalGold;
            SetGoldText();
            DataManager.Instance.SavePlayerData();
            DataManager.Instance.SaveInventoryData();
            ResetSlot();
            detail.gameObject.SetActive(false);
        }
        else {
            SoundManager.Instance.ActingFailSound();
            EventManager.Instance.SelectFish();
            Debug.Log("선택한 물고기가 없습니다.");
        }
    }

    private void ResetSlot() {
        foreach(GameObject gameObject in activeIcon) {
                Destroy(gameObject);
            }
            activeIcon.Clear();
            selectedFish.Clear();

            totalGold = 0;
            totalGoldText.text = totalGold + " C";
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
                slots[index].GetComponent<Button>().onClick.AddListener(() => SetDetail(fishList[index], index, slots[index].transform.position));
            }
        }
    }

    public void SetGoldText() {
        gold.text = playerData.gold + " C";
    }

    public void DefaultSetting() {
        SetSlots();
        SetGoldText();
    }

    public void CloseWindow() {
        detail.gameObject.SetActive(false);
        ResetSlot();
    }
}
