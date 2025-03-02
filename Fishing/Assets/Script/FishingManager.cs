using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour
{
    [SerializeField] RectTransform fish;
    [SerializeField] RectTransform reel;
    [SerializeField] Slider stamina;

    private PlayerInventory playerInventory;
    private float playerStamina;

    private Dictionary<int, float> fishProbabilities = new();
    private string[] fishRarity = {"일반", "희귀", "에픽", "전설"};
    private int fishID;
    private float fishPower;
    private float fishSpeed;
    private float fishingSpeed;
    private bool isOpening;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !isOpening) {
            fish.anchoredPosition += new Vector2(0f, 1f * fishingSpeed);
            reel.Rotate(0f, 0f, -200f * Time.deltaTime);
            stamina.value -= Time.deltaTime * fishSpeed;
        }
        else {
            fish.anchoredPosition -= new Vector2(0f, 0.7f);
            stamina.value += Time.deltaTime * 0.5f;
        }

        fish.anchoredPosition = new Vector2(fish.anchoredPosition.x, Mathf.Clamp(fish.anchoredPosition.y, -280f, 260f));

        if(fish.anchoredPosition.y >= 260f) {
            StartCoroutine(CloseUISequence());
        }
    }

    private void SetFishProbabilities(int baitLevel) {
        fishProbabilities.Clear();
        switch(baitLevel) {
            case 0:
                fishProbabilities.Add(0, 70f);
                fishProbabilities.Add(1, 30f);
                break;
            case 1:
                fishProbabilities.Add(0, 65f);
                fishProbabilities.Add(1, 25f);
                fishProbabilities.Add(2, 10f);
                break;
            case 2:
                fishProbabilities.Add(0, 60f);
                fishProbabilities.Add(1, 25f);
                fishProbabilities.Add(2, 15f);
                break;
            case 3:
                fishProbabilities.Add(0, 55f);
                fishProbabilities.Add(1, 30f);
                fishProbabilities.Add(2, 10f);
                fishProbabilities.Add(3, 5f);
                break;
        }
    }

    private int SetRandomFish() {
        //레어도 선택"
        string rarity = "";
        float randomPoint = Random.Range(0, 100f);
        Debug.Log("물고기 랜덤 포인트" + randomPoint);
        float currentProbability = 0f;
        foreach(var fish in fishProbabilities) {
            Debug.Log("물고기 가중치" + fish.Key + " : " + fish.Value);
            currentProbability += fish.Value;
            if(randomPoint <= currentProbability) {
                rarity = fishRarity[fish.Key];
                break;
            }
        }
        Debug.Log("물고기 레어도" + rarity);
        List<int> fishList = DataManager.Instance.GetFishIDFromList(rarity);

        if(fishList != null && fishList.Count > 0) {
            int randomIndex = Random.Range(0, fishList.Count);
            Debug.Log("물고기 아이디" + fishList[randomIndex]);
            return fishList[randomIndex];
        }
        return 0;
    }

    public void ResetStatus(PlayerData playerData, PlayerInventory _playerInventory)
    {
        SetFishProbabilities(3);
        playerInventory = _playerInventory;
        playerStamina = playerData.stamina;
        animator = GetComponent<Animator>();
        fishID = SetRandomFish();
        fishPower = DataManager.Instance.GetFishPowerFromList(fishID);
        fishSpeed = 1 + (fishPower / 100f);
        fishingSpeed = 1 + (playerInventory.GetRodPower() - fishPower) / 100f;
        Debug.Log("물고기 파워 : " + fishPower);
        fish.anchoredPosition = new Vector2(0f, -280f);
        reel.rotation = Quaternion.Euler(0f, 0f, 0f);
        stamina.maxValue = playerStamina;
        stamina.value = playerStamina;
        StartCoroutine(OpenUISequence());
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
        playerInventory.GetFish(fishID);
        isOpening = true;
        gameObject.SetActive(false);
        EventManager.Instance.EndFishing();
    }
}
