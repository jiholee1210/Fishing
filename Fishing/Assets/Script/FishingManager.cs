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
    [SerializeField] Image grade;

    [SerializeField] GameObject noteItemPrefab;
    [SerializeField] Transform noteArea;

    [SerializeField] PlayerActing playerActing;

    private Dictionary<int, float> fishProbabilities = new();
    private Dictionary<int, float> gradeProbabilities = new();

    private List<FishData> fishList;
    private List<int> relicList;

    private int fishID;
    private int fishGrade;
    private float fishHealth;
    private float fishCurHealth;
    private float fishSpeed;
    private float fishWeight;
    private float fallSpeed;

    private float playerPower;
    private float playerSpeed;
    private float biteTime;
    private float upPos;

    private bool isFishing = false;

    private Animator animator;
    private RectTransform fishRect;

    private Coroutine fishingCoroutine;
    private Coroutine noteCoroutine;
    private Queue<GameObject> noteQueue = new();

    void Awake()
    {
        fishRect = fish.GetComponent<RectTransform>();
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
        if(isFishing) {
            reel.GetComponent<RectTransform>().Rotate(0f, 0f, fallSpeed * Time.deltaTime);

            if(Input.GetMouseButtonDown(0)) {
                if(noteQueue.Count > 0) {
                    if(noteQueue.Peek().GetComponent<RectTransform>().anchoredPosition.y <= -118 && noteQueue.Peek().GetComponent<RectTransform>().anchoredPosition.y >= -164) {
                        UpFish();
                    }
                    else {
                        ReduceDur();
                    }
                }
            }
            if(noteQueue.Count > 0 && noteQueue.Peek().GetComponent<RectTransform>().anchoredPosition.y <= -210) {
                ReduceDur();
                Destroy(noteQueue.Dequeue());
            }
        }   
    }

    private IEnumerator GenNote() {
        // 랜덤 노트 생성
        GameObject note; 
    
        float genTime = 0;
        float reduceTime = 1.4f - (fishGrade * 0.3f);
        fallSpeed = fishSpeed / (fishSpeed + playerSpeed) * 1000f;
        fallSpeed = Mathf.Clamp(fallSpeed, 200f, fallSpeed);
        while(isFishing) {
            genTime = UnityEngine.Random.Range(reduceTime, reduceTime * 2);

            yield return new WaitForSeconds(genTime);
            note = Instantiate(noteItemPrefab, noteArea);
            note.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 220f);
            noteQueue.Enqueue(note);

            Note noteSetting = note.GetComponent<Note>();
            noteSetting.fallSpeed = fallSpeed;
        }
    }

    public void UpFish() {
        // 총 540f 크기 중 현재체력 / 최대체력 비율로 위치 설정
        fishCurHealth -= playerPower;
        Debug.Log("현재 체력 : " + fishCurHealth + " 최대 체력 : " + fishHealth);

        fishRect.anchoredPosition += new Vector2(0f, upPos);
        fishRect.anchoredPosition = new Vector2(0f, Mathf.Clamp(fishRect.anchoredPosition.y, -250f, 290f));

        Vector3 curPos = fishRect.anchoredPosition;

        Destroy(noteQueue.Peek());
        noteQueue.Dequeue();
        Debug.Log("정확하게 누름");
    
        // 물고기 체력 0 이하 체크
        if(fishCurHealth <= 0) {
            StartCoroutine(CloseUISequence());
        }

        StartCoroutine(ShakeFish(curPos));
    }

    private IEnumerator ShakeFish(Vector3 pos) {
        float time = 0f;
        float shakeAngle = 15f;

        Vector3 pivot = fishRect.position + Vector3.up * 30f;
        Debug.Log(pivot);
        while(time <= 0.3f) {
            time += Time.deltaTime;

            float angle = Mathf.Sin(time * 60f) * shakeAngle;
            fishRect.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        fishRect.rotation = Quaternion.Euler(0f, 0f, 0f);
        fishRect.anchoredPosition = pos;
    }

    public void ReduceDur() {
        durability.value -= durability.maxValue * 0.1f + fishWeight / 2;

        StartCoroutine(ShakeBar());
        if(durability.value <= 0) {
            StartCoroutine(FishingFail());
        }
        Debug.Log("잘못 누름");
    }

    private IEnumerator ShakeBar() {
        float time = 0;
        float shakeAmount = 10f;
        durability.transform.GetChild(3).GetChild(0).GetComponent<Image>().color = new Color(1f, 0f, 0f);

        while(time <= 0.3f) {
            time += Time.deltaTime;
            float xPos = Mathf.Sin(time * 60f) * shakeAmount;
            durability.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, durability.GetComponent<RectTransform>().anchoredPosition.y);

            yield return null;
        }
        durability.transform.GetChild(3).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
        durability.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, durability.GetComponent<RectTransform>().anchoredPosition.y);
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
        fishGrade = fishID == 50 ? 3 : SetFishGrade();
        grade.sprite = DataManager.Instance.gradeSprites[fishGrade];
        SetFishStat();
        SetPlayerStat();
        
        fishRect.anchoredPosition = new Vector2(0f, -250f);
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
        float randomTime = UnityEngine.Random.Range(5f, 15f) * (1 - (biteTime / 100));
        while(time < randomTime) {
            time += Time.deltaTime;
            yield return null;
        }
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

        fishHealth = fish.power * (1 + (fishGrade * 0.5f)); // 물고기 체력
        fishCurHealth = fishHealth;

        fishSpeed = fish.speed * (1 + (fishGrade * 0.5f)); // 노트 떨어지는 속도

        float randomWeight = UnityEngine.Random.Range(fish.weightMin, fish.weightMax);
        fishWeight = float.Parse(randomWeight.ToString("F2")); // 내구도 삭제량 관련
    }

    public void SetPlayerStat() {
        durability.maxValue = playerActing.playerInventory.GetRodDur();
        durability.value = durability.maxValue;

        playerPower = playerActing.playerInventory.GetWirePower();
        playerSpeed = playerActing.playerInventory.GetReelSpeed();
        biteTime = playerActing.playerInventory.GetHookPower();
        upPos = 540 * (playerPower / fishHealth);
    }

    IEnumerator OpenUISequence() {
        animator.Play("Window_Open");
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1f);
        isFishing = true;

        noteCoroutine = StartCoroutine(GenNote());
    }

    IEnumerator CloseUISequence() {
        isFishing = false;
        StopCoroutine(noteCoroutine);
        noteCoroutine = null;

        animator.Play("Window_Close");
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

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
        foreach(GameObject gameObject in noteQueue) {
            Debug.Log("노트 삭제");
            Destroy(gameObject);
        }
        noteQueue.Clear();

        playerActing.SetStartFishing();
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

        playerActing.playerInventory.GetFish(id, relicWeight, 3);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EndFishing();
        chest.GetChild(2).gameObject.SetActive(false);
        chest.gameObject.SetActive(false);
    }

    IEnumerator ShowDetail() {
        if(!DataManager.Instance.guide.fishID[fishID] || !DataManager.Instance.guide.fishGrade[fishID].grade[fishGrade]) {
            detail.GetChild(6).gameObject.SetActive(true);
            DataManager.Instance.guide.fishID[fishID] = true;
            DataManager.Instance.guide.fishGrade[fishID].grade[fishGrade] = true;
            DataManager.Instance.SaveGuideData();
        }
        else {
            detail.GetChild(6).gameObject.SetActive(false);
        }
        
        yield return StartCoroutine(FirstFishing());
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
        detail.GetChild(4).GetComponent<Image>().sprite = DataManager.Instance.gradeSprites[fishGrade];
        detail.GetChild(5).GetComponent<TMP_Text>().text = fishWeight + " kg";
        
        animator.Play("Detail_Open");
        yield return null;
        
        float len = animator.GetCurrentAnimatorStateInfo(0).length + 2f;
        yield return new WaitForSeconds(len);

        animator.Play("Detail_Close");
        yield return null;

        len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        detail.gameObject.SetActive(false);
    }

    IEnumerator FishingFail()
    {
        isFishing = false;
        StopCoroutine(noteCoroutine);
        noteCoroutine = null;
        
        animator.Play("Window_Close");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        
        transform.GetChild(0).gameObject.SetActive(false);
        EndFishing();
    }
}
