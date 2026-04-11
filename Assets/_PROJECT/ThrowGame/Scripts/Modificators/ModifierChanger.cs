using System;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class ModifierChanger : UsableItemBase {
    [SerializeReference] ModifierItemSO _throwableModifier;
    [SerializeField] private GameObject _pointerToModifier;
    [SerializeField] private TextMeshProUGUI _modifierNameText;
    
    public IThrowableModifier Modifier => _throwableModifier.Modifier;
    
    [Inject] ModifierManager _modifierManager;
    [Inject] LocalizationData _localization;

    private void Start() {
        _modifierNameText.text =
            _localization.GetTranslatedText(_throwableModifier.Id, _localization.ModifiersTranslate);
    }

    public override void TryUse() {
        if (!IsAvailable) {
            Debug.Log("Модификатор на перезарядке именно что");
            return;
        }
        _modifierManager.TrySetModifier(_throwableModifier.Modifier, this);
    }

    public void ShowPointer() {
        _pointerToModifier.ActiveSelf();
    }
    
    public void HidePointer() {
        _pointerToModifier.DisactiveSelf();
    }
}
