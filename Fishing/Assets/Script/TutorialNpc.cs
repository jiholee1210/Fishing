using System.Collections.Generic;
using UnityEngine;

public class TutorialNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int npcType;
    private string line;
    private List<QuestData> questList;


    public int GetNpcType()
    {
        return npcType;
    }

    public void SetNpcType()
    {
        npcType = 3;
    }

    public string GetLine() {
        return line;
    }

    public List<QuestData> GetQuestList() {
        return questList;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = "외지인은 오랜만이구만. \n이 앞 마을까지 가고 있다고? \n도착하는데 3일은 걸릴텐데 말이야. \n하루 묵게 해줄테니 잠시 좀 도와주지 않겠나?";
        questList = DataManager.Instance.tutorial;
        SetNpcType();
    }
}
