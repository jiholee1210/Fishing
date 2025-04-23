using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MuseumManager : MonoBehaviour
{
    [SerializeField] Transform[] inventorySlots;
    [SerializeField] private Transform[] rewardList;
    [SerializeField] private Transform detail;
    [SerializeField] private GameObject statue;
    [SerializeField] private TMP_Text count;

    [SerializeField] private GameObject selectIconPrefab;
    [SerializeField] private Transform selectIconParent;

    private List<int> relics = new();
    private List<GameObject> icon = new();

    private List<PlayerFish> playerFish;
    private PlayerData playerData;

    private LocalizedString donateCount = new LocalizedString("DialogTable", "museum_count");

    private int[] donate = {1, 5, 10, 20};
    private Color[] rarityColor = {new Color(0f, 0f, 0f), new Color(0f, 0.6f, 0.9f), new Color(0.7f, 0f, 1f), new Color(1f, 0.3f, 0.1f), new Color(0f, 0.8f, 0.6f)};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
        if(playerData.getStatue) {
            statue.SetActive(true);
        }
    }

    private void SetInventorySlots() {
        for(int i = 0; i < playerFish.Count; i++) {
            int index = i;

            inventorySlots[index].GetComponent<Image>().sprite = null;
            inventorySlots[index].GetChild(0).GetComponent<Image>().sprite = null;
            inventorySlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            inventorySlots[index].GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            inventorySlots[index].GetComponent<Button>().onClick.RemoveAllListeners();

            if(playerFish[i].fishID != -1) {
                FishData fishData = DataManager.Instance.GetFishData(playerFish[index].fishID);
                inventorySlots[index].GetComponent<Image>().sprite = fishData.fishIcon;
                inventorySlots[index].GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[playerFish[index].grade];
                inventorySlots[index].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
                inventorySlots[index].GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

                if(fishData.habitat == Habitat.None) {
                    inventorySlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    inventorySlots[index].GetComponent<Button>().onClick.AddListener(() => SelectRelic(index, inventorySlots[index].transform.position));
                }
            }
        }
    }

    private void SelectRelic(int index, Vector2 pos) {
        SoundManager.Instance.ButtonClick();
        SetDetail(index);
        if(relics.Contains(index)) {
            Debug.Log(index);
            int id = relics.IndexOf(index);
            relics.Remove(index);
            Destroy(icon[id]);
            icon.RemoveAt(id);
        }
        else {
            relics.Add(index);
            Vector2 genPos = pos + new Vector2(20f, 20f);
            GameObject selectIcon = Instantiate(selectIconPrefab, selectIconParent);
            icon.Add(selectIcon);
            selectIcon.transform.position = genPos;
        }
    }

    private void SetDetail(int index) {
        detail.gameObject.SetActive(true);
        FishData fishData = DataManager.Instance.GetFishData(playerFish[index].fishID);
        detail.GetChild(0).GetComponent<TMP_Text>().text = fishData.fishName;
        detail.GetChild(1).GetComponent<Image>().sprite = fishData.fishDetail;
        detail.GetChild(2).GetComponent<TMP_Text>().text = fishData.rarityLocalized;
        detail.GetChild(2).GetComponent<TMP_Text>().color = rarityColor[(int)fishData.rarity];
        detail.GetChild(3).GetComponent<TMP_Text>().text = playerFish[index].weight + " kg";
        detail.GetChild(4).GetComponent<TMP_Text>().text = playerFish[index].price + " " + LocalizationSettings.StringDatabase.GetLocalizedString("DialogTable", "coin");
        detail.GetChild(5).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[playerFish[index].grade];
    }

    private void DonateRelics() {
        if(relics.Count > 0) {
            SoundManager.Instance.ButtonClick();
            playerData.donateCount += relics.Count;

            foreach(int index in relics) {
                playerFish[index] = null;
            }

            DataManager.Instance.SavePlayerData();
            DataManager.Instance.SaveInventoryData();
            CloseDonateUI();
        }
        else {
            Debug.Log("유물을 선택하지 않았습니다.");
        }
    }

    private void SetRewardButton() {
        donateCount.Arguments = new object[] {playerData.donateCount};
        count.text = donateCount.GetLocalizedString();
        
        for(int i = 0; i < rewardList.Length; i++) {
            int index = i;

            rewardList[index].GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            if(playerData.donateCount >= donate[index]) {
                if(playerData.museumComplete.Contains(index)) {
                    rewardList[index].GetChild(2).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                    rewardList[index].GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("DialogTable", "museum_complete");
                    continue;
                }
                rewardList[index].GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f);
                rewardList[index].GetChild(2).GetComponent<Button>().onClick.AddListener(() => GetReward(index));
            }
            else {
                rewardList[index].GetChild(2).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
    }

    private void GetReward(int index) {
        SoundManager.Instance.ButtonClick();
        playerData.museumComplete.Add(index);
        switch(index) {
            case 0:
                playerData.gold += 15000;
                break;
            case 1:
                playerData.getRelicReward = true;
                break;
            case 2:
                //스킨 추가;
                playerData.rodList.Add(4);
                break;
            case 3:
                //동상 오브젝트 활성화
                playerData.getStatue = true;
                statue.SetActive(true);
                break;
        }
        DataManager.Instance.SavePlayerData();
        SetRewardButton();
    }


    private void DefaultSetting() {
        playerFish = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;

        if(playerData.museumComplete.Contains(3)) {
            statue.SetActive(true);
        }

        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => OpenDonateUI());
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OpenRewardUI());
        transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() => DonateRelics());
        transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() => CloseDonateUI());

        transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloseRewardUI());
    }

    public void SetLocked() {
        if(playerData.completeQuest.Contains(2)) {
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
    }

    public void CloseWindow() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

        detail.gameObject.SetActive(false);

        relics.Clear();
        icon.Clear();
        foreach(Transform select in selectIconParent) {
            Destroy(select.gameObject);
        }
    }

    private void OpenDonateUI() {
        SoundManager.Instance.ButtonClick();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        SetInventorySlots();
    }

    private void OpenRewardUI() {
        SoundManager.Instance.ButtonClick();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        SetRewardButton();
    }

    private void CloseDonateUI() {
        SoundManager.Instance.ButtonClick();
        detail.gameObject.SetActive(false);

        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        relics.Clear();
        icon.Clear();
        foreach(Transform select in selectIconParent) {
            Destroy(select.gameObject);
        }
    }

    private void CloseRewardUI() {
        SoundManager.Instance.ButtonClick();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }
}
