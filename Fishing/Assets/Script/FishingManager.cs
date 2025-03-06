using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private int SetRandomFish(List<FishData> fishList) {
        List<int> list = fishList
            .Where(fish => fishProbabilities.Keys.Last() > rarityPriority[fish.rarity])
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

    public void ResetStatus(PlayerData playerData, PlayerInventory _playerInventory, List<FishData> fishList)
    {
        SetFishProbabilities(3);
        playerInventory = _playerInventory;
        playerStamina = playerData.stamina;
        animator = GetComponent<Animator>();
        fishID = SetRandomFish(fishList);

        SetFishStat();
        SetPlayerStat();
        
        Debug.Log("물고기 파워 : " + fishPower);
        fish.anchoredPosition = new Vector2(0f, -280f);
        reel.rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(OpenUISequence());
    }

    public void SetFishStat() {
        fishPower = DataManager.Instance.GetFishPowerFromList(fishID);
        fishSpeed = 1 + (fishPower / 100f);
    }

    public void SetPlayerStat() {
        fishingSpeed = 1 + (playerInventory.GetReelPower() - fishPower) / 100f;
        stamina.maxValue = playerStamina + playerInventory.GetRodDur();
        stamina.value = stamina.maxValue;
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
