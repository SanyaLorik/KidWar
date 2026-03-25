using UnityEngine;
using Zenject;

public class ModifierChanger : MonoBehaviour {
    [SerializeReference, SubclassSelector] IThrowableModifier _throwableModifier;
    
    [Inject] ObjectThrowerCalculator _throwerCalculator;

    public void ChangeModifier() {
        _throwerCalculator.SetModifier(_throwableModifier);
    }
}
