using System;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Localization Data")]
public class LocalizationData : LocalizationDataBase
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
    
    
    
    
    public string GetTranslatedName<TId, TItem>(TId id, IEnumerable<TItem> arr)
        where TItem : IIdName<TId>
    {
        foreach (var item in arr)
        {
            if (EqualityComparer<TId>.Default.Equals(item.Id, id))
                return item.Name;
        }
        return null;
    }
    
}


[Serializable]
public class TaskTranslate : IIdName<TaskType> {
    [field: SerializeField] public TaskType Id { get; set; }
    [TextArea] [SerializeField] private string name;
    public string Name { get => name; set => name = value; }
}

public interface IIdName<T> {
    T Id { get; }
    string Name { get; }
}
