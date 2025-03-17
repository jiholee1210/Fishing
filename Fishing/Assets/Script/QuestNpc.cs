using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int type;
    string line;
    List<QuestData> questList;
    public int GetNpcType()
    {
        return type;
    }

    public void SetNpcType()
    {
        type = 3;
    }

    public string GetLine() {
        return line;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNpcType();
        line = "내가 이 마을의 관리인일세. 마을 문제를 해결해주면 그에 맞는 보상을 지급하겠네.";
        questList = DataManager.Instance.island;
    }

    public List<QuestData> GetQuestList()
    {
        return questList;
    }
}
