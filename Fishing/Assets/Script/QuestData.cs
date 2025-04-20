using System;
using UnityEngine;

[Serializable]
public class QuestRequirement {
    public int fishID;  // FishData 대신 ID만 저장
    public int grade;
}

[Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing/Quest Data")]
public class QuestData : ScriptableObject {
    public int questID;
    public bool isEpic;
    public string questName;
    public string desc;
    public int receive;
    public int complete;
    public int[] nextQuest;
    public QuestRequirement[] requirements;  // 필요한 물고기와 수량
    public int rewardGold;
    public string reward;
}
