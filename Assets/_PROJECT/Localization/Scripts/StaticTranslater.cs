using Architecture_M;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class StaticTranslater : MonoBehaviour
{
    [SerializeField] private StaticTranslation<TextMeshProUGUI>[] _texts;

    [Inject] private LocalizationData _localization;
    [Inject] private IInputActivity a;

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

            text.Data.text = translation.Data;
        }
    }
}
