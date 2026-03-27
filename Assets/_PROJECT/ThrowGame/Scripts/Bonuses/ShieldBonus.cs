using System;

[Serializable]
public class ShieldBonus : IBonus {
    public void Use(IDamageable damageable) {
        damageable.SetShielded(true);
    }
}