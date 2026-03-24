using Architecture_M;
using UnityEngine;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Localization Data")]
public class LocalizationData : LocalizationDataBase
{
    [Header("Для  больших чисел")]
    public string[] Suffixies = {"", "K", "M", "B", "T", "Кв", "Ка"};

    public string[] BotsPhrases;

    public string Timer;
}
