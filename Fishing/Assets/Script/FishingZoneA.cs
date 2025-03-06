using System.Collections.Generic;
using UnityEngine;

public class FishingZoneA : MonoBehaviour, IFishingZone
{
    private List<FishData> fishList;

    public List<FishData> GetFishList()
    {
        return fishList;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishList = new()
        {
            DataManager.Instance.GetFishData(12),
            DataManager.Instance.GetFishData(13),
            DataManager.Instance.GetFishData(14),
            DataManager.Instance.GetFishData(15)
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
