using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishFarmManager : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;

    [SerializeField] Transform main;
    [SerializeField] Transform[] fishFarmType;
    [SerializeField] Transform detail;
    [SerializeField] Transform select;
    [SerializeField] GameObject selectIconPrefab;
    [SerializeField] Transform selectIconParent;

    [SerializeField] TMP_Text goldText;

    [SerializeField] GameObject[] fishFarmSlots;
    [SerializeField] GameObject[] fishInvenSlots;


    private List<PlayerFish> fishList;
    private NewFish[] newFishList;
    private List<PlayerFish> fishInFarm;
    private PlayerData playerData;
    private FishFarmTimer[] fishFarmTimer;

    private List<PlayerFish> selectedFish = new();
    private List<int> prevID = new();

    private Coroutine[] coroutines = new Coroutine[12];

    private List<GameObject> activeIcons = new();

    WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultSetting();
        int cid = 0;
        for(int i = 0; i < 4; i++) {
            int groundType = i;
            for(int j = 0; j < 3; j++) {
                int slotIndex = j;
                coroutines[cid++] = StartCoroutine(GenFish(groundType, slotIndex));
            }
        }
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

            fishFarmSlots[index].GetComponent<Image>().sprite = null;
            fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
            fishFarmSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishFarmSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            fishFarmSlots[index].GetComponent<Button>().onClick.AddListener(() => GetFish(groundType, index));
            fishFarmSlots[index].GetComponent<Button>().enabled = false;

            if(newFishList[groundType].list[index].fishID != -1) {
                fishFarmSlots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(newFishList[groundType].list[index].fishID).fishIcon;
                fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[newFishList[groundType].list[index].grade];
                // 아이템에 포인터 올리면 강조 표시 (슬롯에 pointer 관련 인터페이스로 구현) + 누르면 가져오기 (newfishlist에서 삭제하고 플레이어 인벤토리로)
                fishFarmSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                fishFarmSlots[index].GetComponent<Button>().enabled = true;
            }
        }
        SetGoldText(groundType);

        detail.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        detail.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CloseFarmDetail());

        detail.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        detail.GetChild(2).GetComponent<Button>().onClick.AddListener(() => SellFishInBulk(groundType));
    } 

    private void SetFishInFarm(int groundType, int id) {
        int count = groundType * 6 + id * 2;
        int index = id;
        for(int j = 0; j < 2; j++) {
            Transform fishSlot = detail.GetChild(0).GetChild(index).GetChild(j);
            fishSlot.GetComponent<Image>().sprite = null;
            fishSlot.GetChild(0).GetComponent<Image>().sprite = null;

            if(fishInFarm[count].fishID != -1) {
                fishSlot.GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishInFarm[count].fishID).fishIcon;
                fishSlot.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishInFarm[count++].grade];
            }
            else {
                count++;
            }
        }
        detail.GetChild(0).GetChild(index).GetComponent<Button>().onClick.RemoveAllListeners();
        detail.GetChild(0).GetChild(index).GetComponent<Button>().onClick.AddListener(() => OpenFishSlotSetting(groundType, index));
    }

    private IEnumerator GenFish(int groundType, int id) {
        //등급 당 2분 레어도 당 2분 => 합산 시간만큼 기다려야 생산
        int fishFarmIndex = groundType * 6 + id * 2;
        int timeIndex = groundType * 3 + id;

        float genTime = 0f;
        int[] grade = new int[2];
        int fishID = fishInFarm[fishFarmIndex].fishID;

        if(fishID == -1) yield break;

        for(int i = 0; i < 2; i++) {
            grade[i] = fishInFarm[fishFarmIndex + i].grade;
            genTime += (grade[i] + 1) * 120f + 
                       ((int)DataManager.Instance.GetFishData(fishInFarm[fishFarmIndex].fishID).rarity + 1) * 120f;
        }
        float curTime = fishFarmTimer[timeIndex].timer;
        bool isFull = false;
        Debug.Log("물고기 생성 : " +  timeIndex);
        while(fishFarmTimer[timeIndex].isFullFarm) {
            yield return waitOneSecond;
            Debug.Log("생산중 : " + curTime + " " + genTime);

            isFull = newFishList[groundType].list.All(x => x.fishID != -1);

            if(isFull) {
                continue;
            }
            if(curTime >= genTime) {
                for(int i = 0; i < newFishList[groundType].list.Count; i++) {
                    if(newFishList[groundType].list[i].fishID == -1) {
                        
                        PlayerFish newFish = SetNewFish(grade, fishID);
                        FishData fishData = DataManager.Instance.GetFishData(newFish.fishID);
                        newFishList[groundType].list[i] = newFish;//새로운 playerfish 생성
                        
                        fishFarmSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[newFish.grade];
                        fishFarmSlots[i].GetComponent<Image>().sprite = fishData.fishIcon;
                        fishFarmSlots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f ,1f);
                        fishFarmSlots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f ,1f);
                        fishFarmSlots[i].GetComponent<Button>().enabled = true;

                        SetGoldText(groundType);

                        curTime = 0f;
                        break;
                    }
                    if(i == newFishList[groundType].list.Count - 1) {
                        continue;
                    }
                }
            } else {
                curTime += 1f;
            }
            fishFarmTimer[timeIndex].timer = curTime;  
        }
        Debug.Log("생산 종료");
    }

    private void SetGoldText(int groundType) {
        int gold = 0;

        for(int i = 0; i < newFishList[groundType].list.Count; i++) {
            gold += newFishList[groundType].list[i].price;
        }
        goldText.text = gold + " C";
    }

    private PlayerFish SetNewFish(int[] grade, int fishID) {
        FishData fish = DataManager.Instance.GetFishData(fishID);

        int newGrade = grade.Min();
        float randomWeight = UnityEngine.Random.Range(fish.weightMin, fish.weightMax);
        float fishWeight = float.Parse(randomWeight.ToString("F2"));

        return new PlayerFish{
            fishID = fishID,
            grade = newGrade,
            weight = fishWeight,
            price = (int)(fish.price * (fishWeight / fish.weightMin) * (newGrade + 1))
        };
    }
        

    private void GetFish(int groundType, int index) {
        playerInventory.GetFish(newFishList[groundType].list[index]);
        newFishList[groundType].list[index] = null;

        fishFarmSlots[index].GetComponent<Image>().sprite = null;
        fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
        fishFarmSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        fishFarmSlots[index].GetComponent<Button>().enabled = false;

        DataManager.Instance.SaveInventoryData();
    }

    private void SellFishInBulk(int groundType) {
        int gold = 0;
        for(int i = 0; i < newFishList[groundType].list.Count; i++) {
            int index = i;

            gold += newFishList[groundType].list[index].price;

            newFishList[groundType].list[index] = null;

            fishFarmSlots[index].GetComponent<Image>().sprite = null;
            fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
            fishFarmSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishFarmSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishFarmSlots[index].GetComponent<Button>().enabled = false;
        }
        playerData.gold += gold;

        DataManager.Instance.SavePlayerData();
        DataManager.Instance.SaveInventoryData();
    }

    public void OpenFishSlotSetting(int groundType, int id) {
        select.gameObject.SetActive(true);
        select.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        detail.gameObject.SetActive(false);

        for(int i = 0; i < fishInvenSlots.Length; i++) {
            int index = i;
            fishInvenSlots[index].GetComponent<Image>().sprite = null;
            fishInvenSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
            fishInvenSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishInvenSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fishInvenSlots[index].GetComponent<Button>().onClick.RemoveAllListeners();
            fishInvenSlots[index].GetComponent<Button>().enabled = false;

            if(fishList[index].fishID != -1) {
                fishInvenSlots[index].GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishList[index].fishID).fishIcon;
                fishInvenSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishList[index].grade];
                fishInvenSlots[index].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
                fishInvenSlots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                if(DataManager.Instance.GetFishData(fishList[index].fishID).habitat == (Habitat)groundType+1) {
                    fishInvenSlots[index].GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    // 아이템에 포인터 올리면 강조 표시 (슬롯에 pointer 관련 인터페이스로 구현) + 누르면 선택 (2개 누르면 확정 버튼 활성화)
                    // fishInvenSlot만 드래그 가능하게
                    fishInvenSlots[index].GetComponent<Button>().onClick.AddListener(() => SelectFish(fishList[index], index, fishInvenSlots[index].transform.position));
                    fishInvenSlots[index].GetComponent<Button>().enabled = true;
                }
            }
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

    private void SelectFish(PlayerFish playerFish, int id, Vector2 pos) {
        if(!prevID.Contains(id) && selectedFish.Count < 2) {
            selectedFish.Add(playerFish.Clone());
            prevID.Add(id);
            if(prevID.Count == 1) {
                InactiveOtherFish(playerFish.fishID);
            }
            GameObject icon = Instantiate(selectIconPrefab, selectIconParent);
            icon.transform.position = new Vector2(pos.x + 20f, pos.y + 20f);
            activeIcons.Add(icon);
            if(selectedFish.Count >= 2) {
                select.GetChild(0).GetChild(2).GetComponent<Button>().enabled = true;
                select.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        } 
        else if(prevID.Contains(id)) {
            int index = prevID.IndexOf(id);
            selectedFish.RemoveAt(index);
            prevID.Remove(id);
            Debug.Log("재선택");
            Destroy(activeIcons[index]);
            activeIcons.RemoveAt(index);
            if(prevID.Count == 0){
                ActiveSameFish(playerFish.fishID);
            }
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
        rarity.text = fishData.rarity.ToString();
        desc.text = fishData.desc;
        weight.text = playerFish.weight + "kg";
        price.text = playerFish.price + " C";

        image.sprite = fishData.fishDetail;
        image.SetNativeSize();
        grade.sprite = DataManager.Instance.gradeSprites[playerFish.grade];
    }

    private void ActiveSameFish(int fishID) {
        for(int i = 0; i < fishList.Count; i++) {
            if(fishList[i].fishID != -1 && DataManager.Instance.GetFishData(fishID).habitat == DataManager.Instance.GetFishData(fishList[i].fishID).habitat) {
                fishInvenSlots[i].GetComponent<Button>().enabled = true;
                fishInvenSlots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
    }

    private void InactiveOtherFish(int fishID) {
        for(int i = 0; i < fishList.Count; i++) {
            if(fishList[i].fishID != -1 && fishList[i].fishID != fishID) {
                fishInvenSlots[i].GetComponent<Button>().enabled = false;
                fishInvenSlots[i].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
    }

    private void ChangeFishInFarm(int groundType, int id) {
        //selectedFish를 FishinFarm에 그대로 복사 후 리스트 초기화 이후 UI 초기화
        int count = groundType * 6 + id * 2;
        int timerIndex = groundType * 3 + id;

        fishList[prevID[0]] = null;
        fishList[prevID[1]] = null;

        for(int i = 0; i < 2; i++) {
            if(fishInFarm[i + count].fishID == -1) break;
            fishList[prevID[i]] = fishInFarm[i + count].Clone();
        }

        for(int i = 0; i < 2; i++) {
            fishInFarm[i + count] = selectedFish[i].Clone();
        }

        foreach(GameObject gameObject in activeIcons) {
            Destroy(gameObject);
        }
        activeIcons.Clear();

        Debug.Log("타이머 인덱스 " + timerIndex);
        fishFarmTimer[timerIndex].isFullFarm = true;
        fishFarmTimer[timerIndex].timer = 0f;
        if(coroutines[timerIndex] != null) {
            StopCoroutine(coroutines[timerIndex]);
        }
        coroutines[timerIndex] = StartCoroutine(GenFish(groundType, id));
        CloseFishSlotSetting();
        SetFishInFarm(groundType, id);
        DataManager.Instance.SaveInventoryData();
    }

    private void CollectFish(int groundType, int id) {
        int count = groundType * 6 + id * 2;
        int timerIndex = groundType * 3 + id;
        for(int i = count; i < count + 2; i++) {
            for(int j = 0; j < fishList.Count; j++) {
                if(fishList[j].fishID == -1) {
                    fishList[j] = fishInFarm[i].Clone();
                    fishInFarm[i] = null;
                    break;
                }
                if(j == fishList.Count - 1 && i == count + 1) {
                    Debug.Log("낚시 가방이 꽉 찼습니다.");
                    return;
                }
            }
        }
        
        Debug.Log("타이머 인덱스 회수 : "+ timerIndex);
        fishFarmTimer[timerIndex].isFullFarm = false;
        fishFarmTimer[timerIndex].timer = 0f;
        coroutines[timerIndex] = null;
        DataManager.Instance.SaveInventoryData();
        CloseFishSlotSetting();
        SetFishInFarm(groundType, id);
    }

    public void CloseFarmDetail() {
        detail.gameObject.SetActive(false);
        main.gameObject.SetActive(true);

        SetMainSlot();
    }

    public void CloseFishSlotSetting() {
        select.gameObject.SetActive(false);
        detail.gameObject.SetActive(true);

        selectedFish.Clear();
        prevID.Clear();
    }

    private void DefaultSetting() {
        fishList = DataManager.Instance.inventory.fishList;
        playerData = DataManager.Instance.playerData;
        newFishList = DataManager.Instance.inventory.newFishList;
        fishInFarm = DataManager.Instance.inventory.fishInFarm;
        fishFarmTimer = DataManager.Instance.inventory.fishFarmTimer;
    }

    public void SetMainSlot() {
        for(int i = 0; i < playerData.farmUnlock.Length; i++) {
            if(playerData.farmUnlock[i]) {
                fishFarmType[i].GetChild(5).gameObject.SetActive(false);
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
                    Debug.Log(count);
                    if(fishInFarm[count].fishID != -1) {
                        fishSlot.GetComponent<Image>().sprite = DataManager.Instance.GetFishData(fishInFarm[count].fishID).fishIcon;
                        fishSlot.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishInFarm[count++].grade];
                    }
                    else {
                        count++;
                    }
                    
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
