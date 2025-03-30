using System.Collections.Generic;
using UnityEngine;

public class FishingZone : MonoBehaviour, IFishingZone
{
    [SerializeField] int[] fishIDList;
    private List<FishData> fishList;

    void Start()
    {
        fishList = new();
        foreach(int id in fishIDList) {
            FishData fishData = DataManager.Instance.GetFishData(id);
            if(fishData != null) {
                fishList.Add(DataManager.Instance.GetFishData(id));
            }
            else {
                Debug.Log("ID " + id + " 물고기 검색 실패" );
            }
            
        }
    }

    public List<FishData> GetFishList()
    {
        return fishList;
    }
}
