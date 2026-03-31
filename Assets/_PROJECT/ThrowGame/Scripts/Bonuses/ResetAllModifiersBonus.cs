using System;
using Zenject;

[Serializable]
public class ResetAllModifiersBonus : IBonus {

    [Inject] private ModifierManager _modifierManager;
    
    public void Use(IDamageable damageable) {
        _modifierManager.ResetAllPlayerModifiers();
    }
}