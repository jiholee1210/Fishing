using System;
using UnityEngine;

[Serializable]
public class QuestRequirement {
    public int fishID;  // FishData 대신 ID만 저장
    public int requiredAmount;
}

[Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing/Quest Data")]
public class QuestData : ScriptableObject {
    public int questID;
    public string questName;
    public string desc;
    public int[] nextQuest;
    public QuestRequirement[] requirements;  // 필요한 물고기와 수량
    public int rewardGold;
    public int[] rewardItem;
}
