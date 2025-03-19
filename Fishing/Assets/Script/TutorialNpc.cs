using System.Collections.Generic;
using UnityEngine;

public class TutorialNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int npcType;
    private string line;
    private string highlight;
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
        line = "외지인은 오랜만이구만. 무슨 일인가?";
        highlight = "대화하기";
        questList = DataManager.Instance.tutorial;
        Debug.Log("퀘스트 크기" + questList.Count);
        SetNpcType();
    }

    public string GetHighlighter()
    {
        return highlight;
    }
}
