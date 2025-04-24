using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

public class FishingZone : MonoBehaviour, IFishingZone
{
    [SerializeField] int[] fishIDList;
    private List<FishData> fishList;

    private LocalizedString localizedString = new LocalizedString("DialogTable", "highlight_fish");
    private string highlight;

    async void Start()
    {
        await DataManager.Instance.WaitForFishData();

        fishList = new();
        foreach(int id in fishIDList) {
            FishData fishData = DataManager.Instance.GetFishData(id);
            if(fishData != null) {
                fishList.Add(DataManager.Instance.GetFishData(id));
            }
        }
        highlight = localizedString.GetLocalizedString();
    }

    public List<FishData> GetFishList()
    {
        return fishList;
    }

    public string GetHighlighter() {
        return highlight;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 3) {
            StartCoroutine(other.GetComponent<PlayerMovement>().FallIntoWater());
        }
    }
}
