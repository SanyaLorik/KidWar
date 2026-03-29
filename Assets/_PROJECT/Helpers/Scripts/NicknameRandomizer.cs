using System;
using System.Linq;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

enum NickType
{
    Simple,
    NameNumber,
    NameLast,
    NameSuffix,
    TwoWords,
    Leet,
    GamerTag
}


public class NicknameRandomizer : IInitializable
{
    private readonly LocalizationData _localizationData;
    
    private string[] _ruMaleFirst;
    private string[] _ruFemaleFirst;
    private string[] _ruMaleLast;
    private string[] _ruFemaleLast;
    private string[] _enMaleFirst;
    private string[] _enFemaleFirst;
    private string[] _enLast;
    
    private string[] _numberCombines;
    
    private string[] _suffix;
    private string[] _memWords;
    private string[] _linkWords;
    
    private bool _isRusLanguage;
    
    
    private float _chanceToMale { get; set; } = .5f;
    private float _chanceToRus { get; set; } = .5f;
    private float _chanceToHaveNumber { get; set; } = .5f;
    private int _maxCharsName { get; set; } = 12;
    
    
    public NicknameRandomizer(LocalizationData localizationData) {
        _localizationData = localizationData;
    }
    
    public void Initialize()
    {
        GetFilesStrings();
        _isRusLanguage = _localizationData.Substitutes.Any(i => i == LanguageEnum.Russian);
    }
    
    
    
    public string GetRandomName()
    {
        bool male = Random.value < _chanceToMale;
        bool rus = Random.value < _chanceToRus;
        if (!_isRusLanguage) {
            rus = false;
        }
        
        string first;
        string last;
    
        if (rus && male)
        {
            first = _ruMaleFirst.GetRandomElement();
            last = _ruMaleLast.GetRandomElement();
        }
        else if (rus)
        {
            first = _ruFemaleFirst.GetRandomElement();
            last = _ruFemaleLast.GetRandomElement();
        }
        else if (male)
        {
            first = _enMaleFirst.GetRandomElement();
            last = _enLast.GetRandomElement();
        }
        else
        {
            first = _enFemaleFirst.GetRandomElement();
            last = _enLast.GetRandomElement();
        }
    
        return BuildNick(first, last);
    }



    private string BuildNick(string first, string last)
    {
        NickType type = GetWeightedNickType();
    
        string nick = type switch
        {
            NickType.Simple => first,
    
            NickType.NameNumber => first + GetNumber(),
    
            NickType.NameLast => first + "_" + last,
    
            NickType.NameSuffix => first + _suffix.GetRandomElement(),
    
            NickType.TwoWords => first + _memWords.GetRandomElement(),
    
            NickType.Leet => ApplyLeet(first),
    
            NickType.GamerTag => BuildGamerTag(first),
    
            _ => first
        };
    
        nick = MutateCase(nick);
    
        return Trim(nick);
    }
    
    
    private string BuildGamerTag(string first)
    {
        int variant = Random.Range(0, 4);
    
        string num = GetNumber();
        string mem = _memWords.GetRandomElement();
        string link = _linkWords.GetRandomElement();
        string suffix = _suffix.GetRandomElement();
    
        return variant switch
        {
            1 => first + "_" + mem,
    
            2 => first + link + mem,
    
            3 => link + "_" + first + "_" + num,
    
            4 => first + "_" + suffix,
    
            _ => first
        };
    }
    
    private NickType GetWeightedNickType()
    {
        // веса (чем больше, тем чаще)
        float[] weights = new float[]
        {
            1f, // Simple
            1f, // NameNumber
            1f, // NameLast
            0.2f, // NameSuffix
            0.8f, // TwoWords
            0.5f, // Leet
            0.3f  // GamerTag (xX_…Xx)
        };
    
        float total = 0f;
        foreach (var w in weights) total += w;
    
        float r = Random.value * total;
        for (int i = 0; i < weights.Length; i++)
        {
            if (r < weights[i])
                return (NickType)i;
            r -= weights[i];
        }
    
        return NickType.Simple;
    }
    
    
    private string GetNumber()
    {
        if (Random.value > _chanceToHaveNumber)
            return "";
    
        return _numberCombines[Random.Range(0, _numberCombines.Length)];
    }
    
    private string ApplyLeet(string name)
    {
        return name
            .Replace("a", "4")
            .Replace("e", "3")
            .Replace("o", "0")
            .Replace("i", "1");
    }
    
    private string MutateCase(string s)
    {
        float r = Random.value;
    
        if (r < 0.2f) return s.ToUpper();
        if (r < 0.4f) return s.ToLower();
        if (r < 0.6f)
        {
            char[] c = s.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (Random.value < 0.5f)
                    c[i] = char.ToUpper(c[i]);
            }
            return new string(c);
        }
    
        return s;
    }
    
    private string Trim(string s)
    {
        int max = _maxCharsName;
    
        if (s.Length <= max)
            return s;
    
        int spaceIndex = Math.Max(s.LastIndexOf(' ', max), s.LastIndexOf('_', max));
        if (spaceIndex > 0)
            return s.Substring(0, spaceIndex);
    
        return s.Substring(0, max);
    }
    
    private string[] LoadFile(string path)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);
    
        return asset.text
            .Replace("\r", "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
    
    
    private void GetFilesStrings() {
        Debug.Log("GetFilesStrings");
        _ruMaleFirst = LoadFile("PlayerNames/ru_male_first");
        _ruMaleLast = LoadFile("PlayerNames/ru_male_last");
        _ruFemaleFirst = LoadFile("PlayerNames/ru_female_first");
        _ruFemaleLast = LoadFile("PlayerNames/ru_female_last");
        _enMaleFirst = LoadFile("PlayerNames/en_male_first");
        _enFemaleFirst = LoadFile("PlayerNames/en_female_first");
        _enLast = LoadFile("PlayerNames/en_last");
        _numberCombines = LoadFile("PlayerNames/number_combine");
        
        // GET
        _suffix = LoadFile("PlayerNames/Extension/suffixes");
        _memWords = LoadFile("PlayerNames/Extension/mem_words");
        _linkWords = LoadFile("PlayerNames/Extension/link_words");
        
    }

}