using UnityEngine;

public class QuestNpc : MonoBehaviour, INPC
{
    [SerializeField] int type;

    public int GetNpcType()
    {
        return type;
    }

    public void SetNpcType()
    {
        type = 3;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNpcType();
    }
}
