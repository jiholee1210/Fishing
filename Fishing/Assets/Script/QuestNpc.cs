using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : MonoBehaviour, INPC, IQuest
{
    [SerializeField] int npcId;
    [SerializeField] string line;
    private int npcID;

    public int GetNpcType()
    {
        return 3;
    }

    public string GetLine() {
        return line;
    }

    public string GetHighlighter()
    {
        return "대화하기";
    }

    public int GetNpcID()
    {
        return npcID;
    }
}
