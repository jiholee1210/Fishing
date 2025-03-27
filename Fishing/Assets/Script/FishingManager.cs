using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour
{
    [SerializeField] GameObject fish;
    [SerializeField] GameObject reel;
    [SerializeField] Slider durability;
    [SerializeField] GameObject getFishIcon;
    [SerializeField] Transform detail;
    [SerializeField] Transform chest;
    [SerializeField] Sprite[] gradeSprites;

    [SerializeField] PlayerActing playerActing;

    private Dictionary<int, float> fishProbabilities = new();
    private Dictionary<int, float> gradeProbabilities = new();
    private Dictionary<string, int> rarityPriority = new Dictionary<string, int>() {
        {"일반", 0},
        {"희귀", 1},
        {"에픽", 2},
        {"전설", 3},
        {"보물", 4}
    };
    private Dictionary<int, string> priorityRarity = new Dictionary<int, string>() {
        {0, "일반"},
        {1, "희귀"},
        {2, "에픽"},
        {3, "전설"},
        {4, "보물"}
    };

    private List<FishData> fishList;
    private List<int> relicList;

    private int fishID;
    private int fishGrade;
    private float fishPower;
    private float fishSpeed;
    private float fishWeight;
    private float fishResist;

    private float durRegen;
    private float fishingSpeed;
    private bool isOpening = true;
    private bool isClosing = false;

    private Animator animator;
    private RectTransform fishRect;

    private float clickDuration = 0f;  // 마우스 클릭 지속 시간
    private float maxClickDuration = 3f;  // 최대 클릭 지속 가능 시간
    private bool isResisting = false;  // 물고기가 저항하는 중인지
    private Image fishImage;  // 물고기 이미지 컴포넌트

    private float resistDuration = 0f;  // 저항 지속 시간
    private float maxResistDuration = 1f;  // 최대 저항 지속 가능 시간

    private Coroutine fishingCoroutine;

    void Awake()
    {
        fishRect = fish.GetComponent<RectTransform>();
        fishImage = fish.GetComponent<Image>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void OnEnable()
    {
        playerActing.OnFishingEnd += StopFishing;
    }

    void OnDisable()
    {
        playerActing.OnFishingEnd -= StopFishing;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !isOpening) {
            clickDuration += Time.deltaTime;
            float resistanceThreshold = maxClickDuration * fishResist;

            if(clickDuration >= resistanceThreshold) {
                if(!isResisting) {
                    StartCoroutine(FishResistance());
                }
                // 저항 중일 때 시간 추적
                if(isResisting) {
                    resistDuration += Time.deltaTime;
                    if(resistDuration >= maxResistDuration && !isClosing) {
                        isClosing = true;
                        StartCoroutine(FishingFail());
                        return;
                    }
                }
            }
            fishRect.anchoredPosition += new Vector2(0f, fishingSpeed);
            reel.GetComponent<RectTransform>().Rotate(0f, 0f, -200f * Time.deltaTime);
            durability.value -= Time.deltaTime * fishPower;
        }
        else {
            // clickDuration을 서서히 감소
            clickDuration = 0f;
            resistDuration = 0f;
            isResisting = false;
            fishImage.color = Color.white;
            fishRect.anchoredPosition -= new Vector2(0f, 0.7f);
            durability.value += Time.deltaTime * durRegen;
        }

        fishRect.anchoredPosition = new Vector2(fishRect.anchoredPosition.x, Mathf.Clamp(fishRect.anchoredPosition.y, -280f, 260f));

        if(fishRect.anchoredPosition.y >= 260f && !isClosing) {
            isClosing = true;
            StartCoroutine(CloseUISequence());
        }
    }

    private void SetFishProbabilities(int baitLevel) {
        fishProbabilities.Clear();
        Debug.Log("미끼 레벨 : " + baitLevel);
        switch(baitLevel) {
            case 1:
                fishProbabilities.Add(0, 70f);
                fishProbabilities.Add(1, 29f);
                fishProbabilities.Add(2, 0.8f);
                fishProbabilities.Add(3, 0.1f);
                fishProbabilities.Add(4, 0.1f);
                break;
            case 2:
                fishProbabilities.Add(0, 65f);
                fishProbabilities.Add(1, 32f);
                fishProbabilities.Add(2, 2f);
                fishProbabilities.Add(3, 0.5f);
                fishProbabilities.Add(4, 0.5f);
                break;
            case 3:
                fishProbabilities.Add(0, 55f);
                fishProbabilities.Add(1, 36f);
                fishProbabilities.Add(2, 5f);
                fishProbabilities.Add(3, 2f);
                fishProbabilities.Add(4, 2f);
                break;
            case 4:
                fishProbabilities.Add(0, 40f);
                fishProbabilities.Add(1, 40f);
                fishProbabilities.Add(2, 10f);
                fishProbabilities.Add(3, 5f);
                fishProbabilities.Add(4, 5f);
                break;
            default:
                break;
        }
    }

    private void SetGradeProbabilities(int baitLevel) {
        gradeProbabilities.Clear();
        switch(baitLevel) {
            case 1:
                gradeProbabilities.Add(0, 100f);
                break;
            case 2:
                gradeProbabilities.Add(0, 70f);
                gradeProbabilities.Add(1, 30f);
                break;
            case 3:
                gradeProbabilities.Add(0, 50f);
                gradeProbabilities.Add(1, 35f);
                gradeProbabilities.Add(2, 15f);
                break;
            case 4:
                gradeProbabilities.Add(0, 30f);
                gradeProbabilities.Add(1, 40f);
                gradeProbabilities.Add(2, 20f);
                gradeProbabilities.Add(3, 10f);
                break;
        }
    }
    

    private int SetRandomFish(List<FishData> fishList) {
        
        List<FishData> fish = new();
        int rarity = 0;
        float randomProb = UnityEngine.Random.Range(0, 100f);
        Debug.Log(randomProb);
        float curProb = 0;
        foreach(var prob in fishProbabilities) {
            curProb += prob.Value;
            if(randomProb <= curProb) {
                rarity = prob.Key;
                break;
            }
        }

        fish = fishList.Where(x => (int)x.rarity == rarity).ToList();
        
        int randomIndex = UnityEngine.Random.Range(0, fish.Count);
        return fish[randomIndex].fishID;
    }

    public void ResetStatus()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        SetFishProbabilities(playerActing.playerInventory.GetBaitLevel());
        SetGradeProbabilities(playerActing.playerInventory.GetBaitLevel());
        playerActing.SetStartFishing();

        fishID = SetRandomFish(fishList);
        fishGrade = SetFishGrade();
        SetFishStat();
        SetPlayerStat();
        
        fishRect.anchoredPosition = new Vector2(0f, -280f);
        reel.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(OpenUISequence());
    }

    private int SetFishGrade() {
        float randomProb = UnityEngine.Random.Range(0, 100f);
        Debug.Log("랜덤 등급 : " + randomProb);
        float curProb = 0f;
        int grade = 0;
        foreach(var prob in gradeProbabilities) {
            curProb += prob.Value;
            if(curProb >= randomProb) {
                grade = prob.Key;
                break;
            }
        }
        return grade;
    }

    public IEnumerator CalFishing() {
        float time = 0f;
        float randomTime = UnityEngine.Random.Range(5f, 20f);
        Debug.Log(randomTime);
        while(time < randomTime) {
            time += Time.deltaTime * (1 + playerActing.playerInventory.GetHookPower() / 500);
            yield return null;
        }
        Debug.Log("입질이 왔습니다");
        yield return StartCoroutine(OpenGetFishUI());
        ResetStatus();
    }
    
    public void StartFishing(List<FishData> _fishList) {
        fishList = _fishList;
        foreach(FishData fish in fishList) {
            if(fish.fishID > 50) {
                relicList.Add(fish.fishID);
            }
        }

        fishingCoroutine = StartCoroutine(CalFishing());
    }
    public void StopFishing() {
        Debug.Log("낚시 중단");
        if(fishingCoroutine != null) {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }
    }

    public IEnumerator OpenGetFishUI() {
        getFishIcon.SetActive(true);
        Animator animator = getFishIcon.GetComponent<Animator>();
        animator.Play("Get_Fish");
        yield return null;
        
        float len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        getFishIcon.SetActive(false);
    }

    public void SetFishStat() {
        // 내구성 관련 : 파워, 속도 관련 : 스피드, 저항 관련 : 무게
        FishData fish = DataManager.Instance.GetFishData(fishID);
        fishPower = fish.power * (1 + (fishGrade * 0.5f));
        fishSpeed = fish.speed * (1 + (fishGrade * 0.5f));
        float randomWeight = UnityEngine.Random.Range(fish.weightMin, fish.weightMax);
        fishWeight = float.Parse(randomWeight.ToString("F2"));
    }

    public void SetPlayerStat() {
        fishingSpeed = Math.Max(1 + (playerActing.playerInventory.GetReelSpeed() - fishSpeed) / 100f, 0.01f);
        Debug.Log("낚시 속도 : " + fishSpeed + " 릴 속도 : " + playerActing.playerInventory.GetReelSpeed());
        fishResist = Math.Max(1 + (playerActing.playerInventory.GetWirePower() - fishWeight) / 100f, 0.33f);
        Debug.Log("낚시 저항 : " + fishResist);
        durability.maxValue = playerActing.playerInventory.GetRodDur();
        durability.value = durability.maxValue;

        durRegen = durability.maxValue * 0.1f;
    }

    IEnumerator OpenFishingUIAnimation() {
        animator.Play("Window_Open");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator OpenUISequence() {
        yield return StartCoroutine(OpenFishingUIAnimation());
        isOpening = false;
    }

    IEnumerator CloseFishingUIAnimation() {
        animator.Play("Window_Close");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator CloseUISequence() {
        yield return StartCoroutine(CloseFishingUIAnimation());
        transform.GetChild(0).gameObject.SetActive(false);
        if(fishID == 50) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            int relicID = relicList[UnityEngine.Random.Range(0, relicList.Count)];
            chest.gameObject.SetActive(true);
            chest.GetComponent<Animator>().Play("ChestPanel_Open");
            chest.GetChild(1).GetComponent<Animator>().Play("Chest_Open");
            chest.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OpenChest(relicID));
        }
        else {
            yield return StartCoroutine(ShowDetail());
            playerActing.playerInventory.GetFish(fishID, fishWeight, fishGrade);
            EndFishing();
        }
    }

    private void EndFishing() {
        isClosing = false;
        playerActing.SetStartFishing();
        isOpening = true;
        isResisting = false;
        fishImage.color = Color.white;
        EventManager.Instance.EndFishing();
    }

    private void OpenChest(int id) {
        chest.GetChild(2).gameObject.SetActive(true);
        chest.GetChild(2).GetComponent<Button>().onClick.AddListener(() => StartCoroutine(CloseChest(id)));

        chest.GetChild(1).GetComponent<Animator>().Play("Chest_Close");

        chest.GetChild(3).GetComponent<Image>().sprite = DataManager.Instance.GetItemData(id).itemImage;
        chest.GetChild(3).GetComponent<Animator>().Play("Inside_Open");
    }

    private IEnumerator CloseChest(int id) {
        chest.GetChild(3).GetComponent<Animator>().Play("Inside_Close");
        chest.GetComponent<Animator>().Play("ChestPanel_Close");
        yield return null;

        float len = chest.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);

        FishData relic = DataManager.Instance.GetFishData(id);
        float randomWeight = UnityEngine.Random.Range(relic.weightMin, relic.weightMax);
        float relicWeight = float.Parse(randomWeight.ToString("F2"));

        playerActing.playerInventory.GetFish(id, relicWeight, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EndFishing();
        chest.GetChild(2).gameObject.SetActive(false);
        chest.gameObject.SetActive(false);
    }

    IEnumerator ShowDetail() {
        if(!DataManager.Instance.guide.fishID[fishID] || !DataManager.Instance.guide.fishGrade[fishID].grade[fishGrade]) {
            DataManager.Instance.guide.fishID[fishID] = true;
            DataManager.Instance.guide.fishGrade[fishID].grade[fishGrade] = true;
            DataManager.Instance.SaveGuideData();
            yield return StartCoroutine(FirstFishing());
        }
        else {
            //yield return StartCoroutine()
        }
    }

    IEnumerator FirstFishing() {
        detail.gameObject.SetActive(true);
        Animator animator = detail.GetComponent<Animator>();
        FishData fish = DataManager.Instance.GetFishData(fishID);

        detail.GetChild(0).GetComponent<TMP_Text>().text = fish.fishName;
        Image image = detail.GetChild(1).GetComponent<Image>();
        image.sprite = fish.fishDetail;
        image.SetNativeSize();
        detail.GetChild(2).GetComponent<TMP_Text>().text = fish.rarity.ToString();
        detail.GetChild(3).GetComponent<TMP_Text>().text = fish.desc;
        detail.GetChild(4).GetComponent<Image>().sprite = gradeSprites[fishGrade];
        
        animator.Play("Detail_Open");
        yield return null;
        
        float len = animator.GetCurrentAnimatorStateInfo(0).length + 1f;
        yield return new WaitForSeconds(len);

        animator.Play("Detail_Close");
        yield return null;

        len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        detail.gameObject.SetActive(false);
    }

    IEnumerator FishResistance()
    {
        isResisting = true;
        float shakeAmount = 5f;  // 흔들림 정도
        
        while(isResisting && Input.GetMouseButton(0)) {
            // 빨간색으로 변경
            fishImage.color = Color.red;
            
            // 좌우 흔들림
            float randomX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            fishRect.anchoredPosition = new Vector2(randomX, fishRect.anchoredPosition.y);
            
            yield return new WaitForSeconds(0.05f);
        }

        // 원래 위치로 복귀
        fishRect.anchoredPosition = new Vector2(0f, fishRect.anchoredPosition.y);
        fishImage.color = Color.white;
    }

    IEnumerator FishingFail()
    {
        animator.Play("Window_Close");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        transform.GetChild(0).gameObject.SetActive(false);
        EndFishing();
    }
}
