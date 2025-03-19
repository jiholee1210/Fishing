using UnityEngine;

public interface INPC : IScannable
{
    void SetNpcType();
    int GetNpcType();    
    string GetLine();
}
