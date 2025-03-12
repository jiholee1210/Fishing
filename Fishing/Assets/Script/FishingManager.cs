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

    private PlayerInventory playerInventory;

    private Dictionary<int, float> fishProbabilities = new();
    private Dictionary<string, int> rarityPriority = new Dictionary<string, int>() {
        {"일반", 0},
        {"희귀", 1},
        {"에픽", 2},
        {"전설", 3}
    };
    private Dictionary<int, string> priorityRarity = new Dictionary<int, string>() {
        {0, "일반"},
        {1, "희귀"},
        {2, "에픽"},
        {3, "전설"}
    };

    private int fishID;
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

    void Awake()
    {
        fishRect = fish.GetComponent<RectTransform>();
        fishImage = fish.GetComponent<Image>();
        animator = transform.GetChild(0).GetComponent<Animator>();
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
                    if(resistDuration >= maxResistDuration) {
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
                fishProbabilities.Add(1, 30f);
                break;
            case 2:
                fishProbabilities.Add(0, 65f);
                fishProbabilities.Add(1, 25f);
                fishProbabilities.Add(2, 10f);
                break;
            case 3:
                fishProbabilities.Add(0, 60f);
                fishProbabilities.Add(1, 25f);
                fishProbabilities.Add(2, 15f);
                break;
            case 4:
                fishProbabilities.Add(0, 55f);
                fishProbabilities.Add(1, 30f);
                fishProbabilities.Add(2, 10f);
                fishProbabilities.Add(3, 5f);
                break;
            default:
                break;
        }
    }

    private int SetRandomFish(List<FishData> fishList) {
        List<int> list = fishList
            .Where(fish => fishProbabilities.Keys.Last() >= rarityPriority[fish.rarity])
            .Select(fish => rarityPriority[fish.rarity])
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        List<FishData> fish = new();
        int rarity = 0;
        float randomProb = UnityEngine.Random.Range(0, 100f);
        Debug.Log(randomProb);
        float curProb = 0;
        foreach(var prob in fishProbabilities) {
            curProb += prob.Value;
            if(randomProb <= curProb) {
                Debug.Log(list.Count);
                for(int j = 0; j < list.Count; j++) {
                    
                    if(list[j] > prob.Key) {
                        Debug.Log(rarity);
                        fish = fishList.Where(f => priorityRarity[rarity] == f.rarity).ToList();
                        break;
                    }
                    if(j == list.Count - 1) {
                        rarity = list[j];
                        Debug.Log(rarity);
                        fish = fishList.Where(f => priorityRarity[rarity] == f.rarity).ToList();
                        break;
                    }
                    rarity = list[j]; 
                }
                break;
            }
        }
        
        int randomIndex = UnityEngine.Random.Range(0, fish.Count);
        return fish[randomIndex].fishID;
    }

    public void ResetStatus(PlayerInventory _playerInventory, List<FishData> fishList)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        playerInventory = _playerInventory;
        SetFishProbabilities(playerInventory.GetBaitLevel());

        fishID = SetRandomFish(fishList);
        SetFishStat();
        SetPlayerStat();
        
        fishRect.anchoredPosition = new Vector2(0f, -280f);
        reel.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(OpenUISequence());
    }

    public IEnumerator CalFishing(PlayerInventory _playerInventory, List<FishData> fishList) {
        float time = 0f;
        float randomTime = UnityEngine.Random.Range(5f, 20f);
        Debug.Log(randomTime);
        while(time < randomTime) {
            time += Time.deltaTime * (1 + _playerInventory.GetHookPower() / 1000);
            yield return null;
        }
        Debug.Log("입질이 왔습니다");
        yield return StartCoroutine(OpenGetFishUI());
        ResetStatus(_playerInventory, fishList);
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
        fishPower = fish.power;
        fishSpeed = fish.speed;
        float randomWeight = UnityEngine.Random.Range(fish.weightMin, fish.weightMax);
        fishWeight = float.Parse(randomWeight.ToString("F2"));
    }

    public void SetPlayerStat() {
        fishingSpeed = Math.Max(1 + (playerInventory.GetReelSpeed() - fishSpeed) / 100f, 0.01f);
        Debug.Log("낚시 속도 : " + fishSpeed + " 릴 속도 : " + playerInventory.GetReelSpeed());
        fishResist = Math.Max(1 + (playerInventory.GetWirePower() - fishWeight) / 100f, 0.33f);
        Debug.Log("낚시 저항 : " + fishResist);
        durability.maxValue = playerInventory.GetRodDur();
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
        yield return StartCoroutine(ShowDetail());
        isClosing = false;
        playerInventory.GetFish(fishID, fishWeight);
        isOpening = true;
        isResisting = false;
        fishImage.color = Color.white;
        transform.GetChild(0).gameObject.SetActive(false);
        EventManager.Instance.EndFishing();
    }

    IEnumerator ShowDetail() {
        if(!DataManager.Instance.guide.fishID[fishID]) {
            DataManager.Instance.guide.fishID[fishID] = true;
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
        detail.GetChild(1).GetComponent<Image>().sprite = fish.fishIcon;
        detail.GetChild(2).GetComponent<TMP_Text>().text = fish.rarity;
        detail.GetChild(3).GetComponent<TMP_Text>().text = fish.desc;
        
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
        isResisting = false;
        fishImage.color = Color.white;
        
        // 실패 애니메이션이나 효과를 여기에 추가할 수 있습니다
        animator.Play("Window_Close");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        isOpening = true;
        transform.GetChild(0).gameObject.SetActive(false);
        EventManager.Instance.EndFishing();
    }
}
