using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] GameObject tooltipObject;

    public void ShowTooltip(int itemID, Vector3 pos) {
        tooltipObject.SetActive(true);
        tooltipObject.transform.position = pos;

        int type = (int)DataManager.Instance.GetItemData(itemID).itemType;
        switch (type) {
            case 0:
                RodData rodData = DataManager.Instance.GetRodData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = rodData.rodName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "내구도 : " + rodData.rodDur;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = "낚시를 지속할 수 있는 시간 증가";
                break;
            case 1:
                ReelData reelData = DataManager.Instance.GetReelData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = reelData.reelName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "스피드 : " + reelData.reelSpeed;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = "물고기를 끌어올리는 속도 증가";
                break;
            case 2:
                WireData wireData = DataManager.Instance.GetWireData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = wireData.wireName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "파워 : " + wireData.wirePower;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = "물고기가 저항하는 힘을 감소";
                break;
            case 3:
                HookData hookData = DataManager.Instance.GetHookData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = hookData.hookName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "파워 : " + hookData.hookPower;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = "물고기가 미끼를 무는 시간 감소";
                break;
            case 4:
                BaitData baitData = DataManager.Instance.GetBaitData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = baitData.baitName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = "미끼 레벨 : " + baitData.baitLevel;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = "높은 레어도 물고기 등장 확률 업 / 더 높은 등급의 물고기 등장";
                break;
        }
    }

    public void HideTooltip() {
        tooltipObject.SetActive(false);
    }
}
