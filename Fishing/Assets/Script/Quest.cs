using UnityEngine;

public class Quest : MonoBehaviour
{
    private QuestData questData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void SetQuest(QuestData _questData) {
        questData = _questData;
    }

    public QuestData GetQuest() {
        return questData;
    }
}
