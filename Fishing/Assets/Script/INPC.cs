using UnityEngine;

public interface INPC : IScannable
{
    int GetNpcType();    
    string GetLine();
}
