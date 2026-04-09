using Architecture_M;
using LuringPlayer_M;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Localization Data")]
public class LocalizationData : LocalizationDataBase,
    IDailyRewardLocalization
{
    [Header("Для  больших чисел")]
    public string[] Suffixies = {"", "K", "M", "B", "T", "Кв", "Ка"};

    public string[] BotsPhrases;

    public string Timer;
    public string Enemy;
    // Battle info
    public string PlayerHit;
    public string PlayerWinner;
    
    // LISTS
    public List<TaskTranslate> TaskTranslates;
    public List<TutorialTranslate> TutorialTranslates;
    public List<BonusesTranslate> BonusesTranslates;

    public DailyRewardLocaliation DailyReward;

    DailyRewardLocaliation IDailyRewardLocalization.DailyReward => DailyReward;

    public string GetTranslatedText<TId, TItem>(TId id, IEnumerable<TItem> arr)
        where TItem : IIdName<TId>
    {
        foreach (var item in arr)
        {
            if (EqualityComparer<TId>.Default.Equals(item.Id, id))
                return item.Text;
        }
        return null;
    }
    
}

public interface IIdName<T> {
    T Id { get; }
    string Text { get; }
}


[Serializable]
public class TaskTranslate : IIdName<TaskType> {
    [field: SerializeField] public TaskType Id { get; set; }
    [TextArea] [SerializeField] private string _text;
    public string Text => _text;
}


[Serializable]
public class TutorialTranslate : IIdName<TutorialStep> {
    [field: SerializeField] public TutorialStep Id { get; set; }
    [TextArea] [SerializeField] private string _text;
    public string Text => _text;
}

[Serializable]
public class BonusesTranslate : IIdName<string> {
    [field: SerializeField] public string Id { get; set; }
    [SerializeField] private string _text;
    public string Text => _text;
}