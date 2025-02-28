using UnityEngine;

public class Merchant : MonoBehaviour, INPC
{
    [SerializeField] int type;

    public void SetNpcType()
    {
        type = 1;
    }

    public int GetNpcType()
    {   
        return type;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNpcType();
    }
}
