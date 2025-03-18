using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignManager : MonoBehaviour
{
    [SerializeField] Button nextButton;
    [SerializeField] Sprite[] tutoSprite;
    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    List<string> desc = new List<string>();
    int index;

    void Start()
    {
        nextButton.onClick.AddListener(() => ShowNextImage());
        desc.Add("낚시 가능 구역에서 클릭으로 낚시를 시작하세요");
        desc.Add("물고기가 미끼를 물면 노란 느낌표가 뜹니다");
        desc.Add("클릭을 유지하면 물고기를 서서히 낚을 수 있습니다");
        desc.Add("물고기가 붉게 변하면 잠시 낚시를 멈춰주세요");
        desc.Add("내구도와 물고기 상태를 잘 관찰하면서 낚시를 즐기세요");
    }

    public void ShowNextImage() {
        index++;
        if(index == 4) {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => EventManager.Instance.CloseSignUI());
        }
        image.sprite = tutoSprite[index];
        text.text = desc[index];
    }

    public void ShowFirstImage() {
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => ShowNextImage());
        index = 0;
        image.sprite = tutoSprite[index];
        text.text = desc[index];
    }
}
