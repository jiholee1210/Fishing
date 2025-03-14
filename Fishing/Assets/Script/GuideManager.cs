using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    [SerializeField] Button[] filters;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject guidePrefab;
    [SerializeField] GameObject detail;
    [SerializeField] Transform parent;
    [SerializeField] Sprite none;

    private readonly Habitat[] habitats = { Habitat.None, Habitat.Freshwater, Habitat.Sea, Habitat.Lava, Habitat.Rock};

    private List<bool> guideList;
    private bool isDetailOpen = false;

    // 버튼별 클릭 이벤트
    // 도감 데이터 상 true 인 물고기 데이터만 출력
    // 필터에 따라 서식지가 동일한 물고기 데이터만 출력

    public void DefaultSetting() {
        guideList = DataManager.Instance.guide.fishID;

        for(int i = 0; i < filters.Length; i++) {
            int index = i;
            filters[index].onClick.AddListener(() => SetGuide(habitats[index]));
        }
        closeButton.onClick.AddListener(() => StartCoroutine(CloseDetail()));
    }

    private void SetGuide(Habitat habitat) {
        foreach(Transform child in parent) {
            Destroy(child.gameObject);
        }

        if(guideList != null) {
            for(int i = 0; i < guideList.Count; i++) {
                FishData fish = DataManager.Instance.GetFishData(i);

                if(habitat == Habitat.None || fish.habitat == habitat) {
                    GameObject fishItem = Instantiate(guidePrefab, parent);
                    if(guideList[i]) {
                        fishItem.transform.GetChild(0).GetComponent<TMP_Text>().text = "No." + (fish.fishID + 1);
                        fishItem.transform.GetChild(1).GetComponent<Image>().sprite = fish.fishIcon;
                        fishItem.transform.GetChild(2).GetComponent<TMP_Text>().text = fish.fishName;
                        fishItem.GetComponent<Button>().onClick.AddListener(() => SetDetail(fish));
                    }
                    else {
                        fishItem.transform.GetChild(0).GetComponent<TMP_Text>().text = "No." + (fish.fishID + 1);
                        fishItem.transform.GetChild(1).GetComponent<Image>().sprite = none;
                        fishItem.transform.GetChild(2).GetComponent<TMP_Text>().text = "???";
                    }
                }
            }
        }
    }

    private void SetDetail(FishData fishData) {
        if(!isDetailOpen) {
            detail.SetActive(true);
            closeButton.gameObject.SetActive(true);

            isDetailOpen = true;

            detail.transform.GetChild(0).GetComponent<TMP_Text>().text = fishData.fishName;
            detail.transform.GetChild(1).GetComponent<TMP_Text>().text = fishData.rarity;
            
            Image fishImage = detail.transform.GetChild(2).GetComponent<Image>();
            fishImage.sprite = fishData.fishDetail;
            fishImage.SetNativeSize();

            detail.transform.GetChild(3).GetComponent<TMP_Text>().text = fishData.weightMin + " ~ " + fishData.weightMax + " kg";
            detail.transform.GetChild(4).GetComponent<TMP_Text>().text = fishData.price + " 코인";
            detail.transform.GetChild(5).GetComponent<TMP_Text>().text = fishData.desc;

            detail.GetComponent<Animator>().Play("Detail_Open");
        }
    }

    public IEnumerator CloseDetail() {
        detail.GetComponent<Animator>().Play("Detail_Close");
        yield return null;
        
        float len = detail.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        isDetailOpen = false;
        detail.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void CloseWindow() {
        foreach(Transform child in parent) {
            Destroy(child.gameObject);
        }
        isDetailOpen = false;
        detail.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CloseWindow();
    }
}


