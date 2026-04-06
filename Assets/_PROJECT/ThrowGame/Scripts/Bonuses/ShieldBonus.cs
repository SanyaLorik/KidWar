using System;
using UnityEngine;

[Serializable]
public class ShieldBonus : IBonus {
    [SerializeField] private int _shieldHp;
    
    public void Use(IDamageable damageable) {
        damageable.EnableShield(_shieldHp);
    }
}