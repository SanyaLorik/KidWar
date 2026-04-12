using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class StaticTranslater : MonoBehaviour
{
    [SerializeField] private List<StaticTranslation<List<TextMeshProUGUI>>> _texts;

    [Inject] private LocalizationData _localization;

    public LocalizationData Header;

    private void Start()
    {
        foreach (var text in _texts)
        {
            var translation = _localization.StaticTranslates.FirstOrDefault(i => i.Id == text.Id);
            if (string.IsNullOrEmpty(translation.Id) || translation.Data == null)
            {
                Debug.LogError($"Нет перевода для {text.Id}");
                continue;
            }

            text.Data.ForEach(i => i.text = translation.Data);
        }
    }

    [ContextMenu("FillHeader")]
    public void FillHeader()
    {
        foreach (var header in Header.StaticTranslates)
        {
            StaticTranslation<List<TextMeshProUGUI>> staticTranslation = new();
            staticTranslation.Id = header.Id;

            _texts.Add(staticTranslation);
        }
    }
}
