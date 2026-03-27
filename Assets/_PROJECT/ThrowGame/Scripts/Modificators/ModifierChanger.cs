using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ModifierChanger : UsableItemBase {
    [SerializeReference, SubclassSelector] IThrowableModifier _throwableModifier;
    [SerializeField] private GameObject _pointerToModifier;
    
    public IThrowableModifier Modifier => _throwableModifier;
    
    [Inject] ModifierManager _modifierManager;

    public override void TryUse() {
        if (!_isAvailable) {
            Debug.Log("Модификатор на перезарядке именно что");
            return;
        }
        _modifierManager.TrySetModifier(_throwableModifier, this);
    }

    public void ShowPointer() {
        _pointerToModifier.ActiveSelf();
    }
    
    public void HidePointer() {
        _pointerToModifier.DisactiveSelf();
    }
}
