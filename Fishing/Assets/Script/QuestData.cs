using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

[Serializable]
public class QuestRequirement {
    public int fishID;  // FishData 대신 ID만 저장
    public int grade;
}

public static class QuestConstants {
    public static readonly string QuestTable = "QuestTable";
}

[Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing/Quest Data")]
public class QuestData : ScriptableObject {
    public int questID;
    public bool isEpic;
    public int receive;
    public int complete;
    public int[] nextQuest;
    public QuestRequirement[] requirements;  // 필요한 물고기와 수량
    public int rewardGold;
    

    public string questNameKey
        => "name" + questID;
    public string questDescKey
        => "desc" + questID;
    public string questRewardKey
        => "reward" + questID;
    public string questName
        => questID == 1001 ? LocalizationSettings.StringDatabase.GetLocalizedString(QuestConstants.QuestTable, "normal_name") : LocalizationSettings.StringDatabase.GetLocalizedString(QuestConstants.QuestTable, questNameKey);
    public string desc
        => questID == 1001 ? LocalizationSettings.StringDatabase.GetLocalizedString(QuestConstants.QuestTable, "normal_desc") : LocalizationSettings.StringDatabase.GetLocalizedString(QuestConstants.QuestTable, questDescKey);
    public string reward
        => LocalizationSettings.StringDatabase.GetLocalizedString(QuestConstants.QuestTable, questRewardKey);
}