using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] GameObject tooltipObject;
    [SerializeField] private GameObject rodToolTip;
    [SerializeField] private GameObject[] rodObject;

    public void ShowTooltip(int itemID, Vector3 pos) {
        tooltipObject.SetActive(true);
        tooltipObject.transform.position = pos;

        int type = (int)DataManager.Instance.GetItemData(itemID).itemType;
        switch (type) {
            case 0:
                RodData rodData = DataManager.Instance.GetRodData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = rodData.rodName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "rod") + rodData.rodDur;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "rod_desc");
                break;
            case 1:
                ReelData reelData = DataManager.Instance.GetReelData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = reelData.reelName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "reel") + reelData.reelSpeed;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "reel_desc");
                break;
            case 2:
                WireData wireData = DataManager.Instance.GetWireData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = wireData.wireName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "wire") + wireData.wirePower;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "wire_desc");
                break;
            case 3:
                HookData hookData = DataManager.Instance.GetHookData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = hookData.hookName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "hook") + hookData.hookPower;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "hook_desc");
                break;
            case 4:
                BaitData baitData = DataManager.Instance.GetBaitData(itemID);
                tooltipObject.transform.GetChild(0).GetComponent<TMP_Text>().text = baitData.baitName;
                tooltipObject.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "bait") + baitData.baitLevel;
                tooltipObject.transform.GetChild(2).GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, "bait_desc");
                break;
        }
    }

    public void HideTooltip() {
        tooltipObject.SetActive(false);
    }

    public void ShowRod(int id, Vector3 pos) {
        rodToolTip.SetActive(true);
        rodToolTip.transform.position = pos;
        rodObject[id].SetActive(true);
    }

    public void HideRod(int id) {
        rodToolTip.SetActive(false);
        rodObject[id].SetActive(false);
    }
}
