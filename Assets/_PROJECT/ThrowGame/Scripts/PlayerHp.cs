using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PlayerHp : MonoBehaviour, IDamageable {
    [SerializeField] private ShieldVisual _shieldVisual;
    private bool _stayInLeft;
    private int _shieldMaxHp;
    public int MaxHp => _data.PlayerMaxHp;
    public int CurrentHp { get; private set; }
    public bool IsInvinsible { get; private set; }
    public bool IsShielded { get; private set; }

    public int CurrentShieldHp { get; private set; }


    [Inject] private HpView _hpView;
    [Inject] private GameData _data;
    [Inject] 

    private void Start() {
        _shieldVisual.gameObject.DisactiveSelf();
    }

    public void InitPosition(bool stayInLeft) {
        _stayInLeft = stayInLeft;
    }
    
    public void AddDamage(int hp) {
        // Debug.Log("IsInvinsible = " + IsInvinsible);
        if (IsInvinsible) {
            // Debug.Log("Сам по себе попал...");
            return;
        }
        if (IsShielded) {
            hp = SetShieldDamage(hp);
        }
        CurrentHp -= hp;
        CurrentHp = Mathf.Max(0, CurrentHp);
        // Debug.Log($"Попал! Минус {hp} хп, щас HP = {CurrentHp} ");
        _hpView.MinusHp(CurrentHp, _stayInLeft);
    }


    public void AddHp(int newHp) {
        int needRecoverHp = MaxHp - CurrentHp;
        newHp = Math.Clamp(newHp, 0, needRecoverHp);
        CurrentHp += newHp;
        CurrentHp = Mathf.Min(CurrentHp, MaxHp);
        Debug.Log($"Добавление хп {newHp} хп, щас HP = {CurrentHp} ");
        _hpView.AddHp(CurrentHp, _stayInLeft, newHp);
    }

    public void SetInvinsible(bool state) {
        IsInvinsible = state;
    }

    public void EnableShield(int shieldHp) {
        // Пока так, каждый раз запоминаем одно и тоже число
        _shieldMaxHp = shieldHp;
        CurrentShieldHp = shieldHp;
        
        // Debug.Log("Включение щита: " + state);
        IsShielded = true;
        _shieldVisual.gameObject.ActiveSelf();
        _shieldVisual.ShieldEnableAnimate(true, CurrentShieldHp);
        _hpView.UsePlayerShield();
        

    }
    
    public void DisableShield() {
        IsShielded = false;
        _shieldVisual.ShieldEnableAnimate(false, 0);
    }

    /// <summary>
    /// Урон сначала проходит по щиту, снимает его хп и возвращает хп сколько снеслось игроку
    /// </summary>
    /// <param name="damage"></param>
    public int SetShieldDamage(int damage) {
        if (damage < CurrentShieldHp) {
            CurrentShieldHp -= damage;
            Debug.Log($"Щит выдержал осталось у щита {CurrentShieldHp}хп");
            _shieldVisual.SetShieldPercentage((float)CurrentShieldHp/_shieldMaxHp, CurrentShieldHp);
            return 0;
        }
        Debug.Log("Щит сломался");
        CurrentShieldHp = 0;
        _shieldVisual.SetShieldPercentage(0, CurrentShieldHp);
        IsShielded = false;
        return damage - CurrentShieldHp;
    }



    public void SetMaxHp() {
        CurrentHp = MaxHp;
        _hpView.SetMaxHp(CurrentHp, _stayInLeft);
    }

    
    public void SetDead() {
        CurrentHp = 0;
    }


}