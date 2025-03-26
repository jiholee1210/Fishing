using UnityEngine;

public class FishFarmNpc : MonoBehaviour, INPC
{
    [SerializeField] string line;

    private int type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        type = 4;
    }
    
    public string GetHighlighter()
    {
        return "대화하기";
    }

    public string GetLine()
    {
        return line;
    }

    public int GetNpcType()
    {
        return type;
    }

    
}
