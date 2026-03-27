using System;
using UnityEngine;

[Serializable]
public class HealBonus : IBonus {
    [SerializeField] private int _healUnits;

    public void Use(IDamageable damageable) {
        damageable.AddHp(_healUnits);
    }
}