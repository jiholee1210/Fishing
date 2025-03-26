using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishFarmManager : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;

    [SerializeField] Transform main;
    [SerializeField] Transform[] fishFarmType;
    [SerializeField] Transform detail;
    [SerializeField] Transform select;

    [SerializeField] GameObject[] fishFarmSlots;
    [SerializeField] GameObject[] fishInvenSlots;


    private List<PlayerFish> fishList;
    private NewFish[] newFishList;
    private List<PlayerFish> fishInFarm;
    private PlayerData playerData;

    private List<PlayerFish> selectedFish = new();
    private List<int> prevID = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
    }
    // 플레이어 데이터 -> 양식장 해금 단계 조회
    // 조회된 단계의 panel만 비활성화(1, 2) 

    public void OpenFarmDetail(int groundType) {
        detail.gameObject.SetActive(true);
        main.gameObject.SetActive(false);

        for(int i = 0; i < 3; i++) {
            SetFishInFarm(groundType, i);
        }
        
        for(int i = 0; i < fishFarmSlots.Length; i++) {
            int index = i;
            if(newFishList[groundType].list[index].fishID == -1) continue;
            fishFarmSlots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(newFishList[groundType].list[index].fishID).fishIcon;
            fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[newFishList[groundType].list[index].grade];
            // 아이템에 포인터 올리면 강조 표시 (슬롯에 pointer 관련 인터페이스로 구현) + 누르면 가져오기 (newfishlist에서 삭제하고 플레이어 인벤토리로)
            fishFarmSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            fishFarmSlots[index].GetComponent<Button>().onClick.AddListener(() => GetFish(groundType, index));
        }

        detail.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        detail.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloseFarmDetail());
    } 

    private void SetFishInFarm(int groundType, int id) {
        int count = groundType * 6 + id * 2;
        int index = id;
        for(int j = 0; j < 2; j++) {
            Transform fishSlot = detail.GetChild(0).GetChild(index).GetChild(j);
            fishSlot.GetComponent<Image>().sprite = null;
            fishSlot.GetChild(0).GetComponent<Image>().sprite = null;

            if(fishInFarm[count].fishID == -1) {
                count++;
                continue;
            }
            
            fishSlot.GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishInFarm[count].fishID).fishIcon;
            fishSlot.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishInFarm[count++].grade];
        }
        detail.GetChild(0).GetChild(index).GetComponent<Button>().onClick.RemoveAllListeners();
        detail.GetChild(0).GetChild(index).GetComponent<Button>().onClick.AddListener(() => OpenFishSlotSetting(groundType, index));
    }

    private void GenFish() {

    }

    private void GetFish(int groundType, int index) {
        playerInventory.GetFish(newFishList[groundType].list[index]);
        newFishList[groundType].list[index] = null;

        fishFarmSlots[index].GetComponent<Image>().sprite = null;
        fishFarmSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void OpenFishSlotSetting(int groundType, int id) {
        select.gameObject.SetActive(true);
        select.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        detail.gameObject.SetActive(false);

        for(int i = 0; i < fishInvenSlots.Length; i++) {
            int index = i;
            fishInvenSlots[index].GetComponent<Image>().sprite = null;
            fishInvenSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();

            if(fishList[index].fishID == -1) continue;
            fishInvenSlots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishList[index].fishID).fishIcon;
            // 아이템에 포인터 올리면 강조 표시 (슬롯에 pointer 관련 인터페이스로 구현) + 누르면 선택 (2개 누르면 확정 버튼 활성화)
            // fishInvenSlot만 드래그 가능하게
            fishInvenSlots[index].GetComponent<Button>().onClick.AddListener(() => SetFishDetail(fishList[index], index));
        }

        Button confirm = select.GetChild(0).GetChild(2).GetComponent<Button>();
        Button exit = select.GetChild(0).GetChild(3).GetComponent<Button>();
        Button collect = select.GetChild(0).GetChild(4).GetComponent<Button>();

        confirm.onClick.RemoveAllListeners();
        exit.onClick.RemoveAllListeners();
        collect.onClick.RemoveAllListeners();

        confirm.onClick.AddListener(() => ChangeFishInFarm(groundType, id));
        confirm.image.color = new Color(0.3f, 0.3f, 0.3f);
        confirm.enabled = false;

        exit.onClick.AddListener(() => CloseFishSlotSetting());

        int count = groundType * 6 + id * 2;

        collect.onClick.AddListener(() => CollectFish(groundType, id));
        collect.image.color = new Color(0.3f, 0.3f, 0.3f);
        collect.enabled = false;

        if(fishInFarm[count].fishID != -1 && fishInFarm[count+1].fishID != -1) {
            collect.image.color = new Color(1f, 1f, 1f);
            collect.enabled = true;
        }
        
    }

    private void SetFishDetail(PlayerFish playerFish, int id) {
        if(!prevID.Contains(id) && selectedFish.Count < 2) {
            selectedFish.Add(playerFish.Clone());
            prevID.Add(id);
            Debug.Log("prevID 배열: " + string.Join(", ", prevID));
            Debug.Log(id + " 번째 물고기 선택 / " + "현재 선택 수 : " + selectedFish.Count);
            if(selectedFish.Count >= 2) {
                select.GetChild(0).GetChild(2).GetComponent<Button>().enabled = true;
                select.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        } 
        else if(prevID.Contains(id)) {
            int index = prevID.IndexOf(id);
            selectedFish.RemoveAt(index);
            prevID.Remove(id);
            Debug.Log("prevID 배열: " + string.Join(", ", prevID));
            Debug.Log(id + " 번째 물고기 선택 / " + "현재 선택 수 : " + selectedFish.Count);
            select.GetChild(0).GetChild(2).GetComponent<Button>().enabled = false;
            select.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
        }
        Transform detailTransform = select.GetChild(0).GetChild(1).GetChild(0);
        detailTransform.gameObject.SetActive(true);
        
        TMP_Text name = detailTransform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rarity = detailTransform.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text desc = detailTransform.GetChild(3).GetComponent<TMP_Text>();
        TMP_Text weight = detailTransform.GetChild(4).GetComponent<TMP_Text>();
        TMP_Text price = detailTransform.GetChild(5).GetComponent<TMP_Text>();

        Image image = detailTransform.GetChild(1).GetComponent<Image>();
        Image grade = detailTransform.GetChild(6).GetComponent<Image>();

        FishData fishData = DataManager.Instance.GetFishData(playerFish.fishID);
        name.text = fishData.fishName;
        rarity.text = fishData.rarity;
        desc.text = fishData.desc;
        weight.text = playerFish.weight + "kg";
        price.text = playerFish.price + " C";

        image.sprite = fishData.fishDetail;
        image.SetNativeSize();
        grade.sprite = DataManager.Instance.gradeSprites[playerFish.grade];
        
    }

    private void ChangeFishInFarm(int groundType, int id) {
        //selectedFish를 FishinFarm에 그대로 복사 후 리스트 초기화 이후 UI 초기화
        int count = groundType * 6 + id * 2;

        fishList[prevID[0]] = null;
        fishList[prevID[1]] = null;

        for(int i = count; i < count + 2; i++) {
            if(fishInFarm[i].fishID == -1) continue;
            for(int j = 0; j < fishList.Count; j++) {
                if(fishList[j].fishID == -1) {
                    fishList[j] = fishInFarm[i].Clone();
                    break;
                }
            }
        }

        for(int i = 0; i < 2; i++) {
            fishInFarm[count++] = selectedFish[i].Clone();
        }

        CloseFishSlotSetting();
        SetFishInFarm(groundType, id);
        DataManager.Instance.SaveInventoryData();
    }

    private void CollectFish(int groundType, int id) {
        int count = groundType * 6 + id * 2;
        for(int i = count; i < count + 2; i++) {
            for(int j = 0; j < fishList.Count; j++) {
                if(fishList[j].fishID == -1) {
                    fishList[j] = fishInFarm[i].Clone();
                    fishInFarm[i] = null;
                    break;
                }
            }
        }
        
        DataManager.Instance.SaveInventoryData();
        CloseFishSlotSetting();
        SetFishInFarm(groundType, id);
    }

    public void CloseFarmDetail() {
        detail.gameObject.SetActive(false);
        main.gameObject.SetActive(true);
    }

    public void CloseFishSlotSetting() {
        select.gameObject.SetActive(false);
        detail.gameObject.SetActive(true);

        SetMainSlot();

        selectedFish.Clear();
        prevID.Clear();
    }

    private void DefaultSetting() {
        fishList = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        newFishList = DataManager.Instance.inventory.newFishList;
        fishInFarm = DataManager.Instance.inventory.fishInFarm;
    }

    public void SetMainSlot() {
        for(int i = 0; i < playerData.farmUnlock.Length; i++) {
            if(playerData.farmUnlock[i]) {
                fishFarmType[i+1].GetChild(5).gameObject.SetActive(false);
            }
        }

        int count = 0;
        for(int i = 0; i < fishFarmType.Length; i++) {
            int index = i;
            for(int j = 0; j < 3; j++) {
                for(int k = 0; k < 2; k++) {
                    Transform fishSlot = fishFarmType[index].GetChild(j).GetChild(k);
                    fishSlot.GetComponent<Image>().sprite = null;
                    fishSlot.GetChild(0).GetComponent<Image>().sprite = null;

                    if(fishInFarm[count].fishID == -1) {
                        count++;
                        continue;
                    }
                    Debug.Log(j  + " " + k);
        
                    fishSlot.GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishInFarm[count].fishID).fishIcon;
                    fishSlot.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishInFarm[count++].grade];
                }
            }
            fishFarmType[index].GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
            fishFarmType[index].GetChild(4).GetComponent<Button>().onClick.AddListener(() => OpenFarmDetail(index));
        }
    }

    public void CloseWindow() {
        main.gameObject.SetActive(true);
        detail.gameObject.SetActive(false);
        select.gameObject.SetActive(false);
    } 
}
