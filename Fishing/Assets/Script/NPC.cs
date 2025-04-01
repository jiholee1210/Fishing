using UnityEngine;

public class NPC : MonoBehaviour, INPC
{
    [SerializeField] private int npcType;
    
    public string GetHighlighter()
    {
        return "대화하기";
    }

    public int GetNpcType()
    {
        return npcType;
    }
}
