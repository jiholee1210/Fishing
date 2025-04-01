using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuseumManager : MonoBehaviour
{
    [SerializeField] Transform[] inventorySlots;
    [SerializeField] private Transform[] rewardList;
    [SerializeField] private Transform detail;

    [SerializeField] private GameObject selectIconPrefab;
    [SerializeField] private Transform selectIconParent;

    private List<int> relics = new();
    private List<GameObject> icon = new();

    private List<PlayerFish> playerFish;
    private PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
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
        detail.GetChild(2).GetComponent<TMP_Text>().text = fishData.rarity.ToString();
        detail.GetChild(3).GetComponent<TMP_Text>().text = playerFish[index].weight + " kg";
        detail.GetChild(4).GetComponent<TMP_Text>().text = playerFish[index].price + " C";
    }

    private void DonateRelics() {
        if(relics.Count > 0) {
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


    private void DefaultSetting() {
        playerFish = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;

        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => OpenDonateUI());
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OpenRewardUI());
        transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() => DonateRelics());
        transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(() => CloseDonateUI());

        transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloseRewardUI());
        foreach(Transform reward in rewardList) {
            reward.GetChild(2).GetComponent<Button>().onClick.AddListener(() => {});
        }
    }

    public void CloseWindow() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }

    private void OpenDonateUI() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        SetInventorySlots();
    }

    private void OpenRewardUI() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    private void CloseDonateUI() {
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
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }
}
