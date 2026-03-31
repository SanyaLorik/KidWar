using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ModifierChanger : UsableItemBase {
    [SerializeReference] ModifierItemSO _throwableModifier;
    [SerializeField] private GameObject _pointerToModifier;
    
    public IThrowableModifier Modifier => _throwableModifier.Modifier;
    
    [Inject] ModifierManager _modifierManager;

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
