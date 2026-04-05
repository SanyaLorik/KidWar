using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PlayerHp : MonoBehaviour, IDamageable {
    [SerializeField] private Transform _shield;
    private bool _stayInLeft;
    public int MaxHp => _data.PlayerMaxHp;
    public int CurrentHp { get; private set; }
    public bool IsInvinsible { get; private set; }
    public bool IsShielded { get; private set; }


    [Inject] private HpView _hpView;
    [Inject] private GameData _data;
    [Inject] private ShieldAnimationLikeBubble _shieldAnimator;

    private void Start() {
        _shield.DisactiveSelf();
    }

    public void InitPosition(bool stayInLeft) {
        _stayInLeft = stayInLeft;
    }
    
    public void AddDamage(int hp) {
        Debug.Log("IsInvinsible = " + IsInvinsible);
        if (IsInvinsible) {
            Debug.Log("Сам по себе попал...");
            return;
        }
        if (IsShielded) {
            SetShielded(false);
            return;
        }
        CurrentHp -= hp;
        CurrentHp = Mathf.Max(0, CurrentHp);
        Debug.Log($"Попал! Минус {hp} хп, щас HP = {CurrentHp} ");
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

    public void SetShielded(bool enable) {
        // Debug.Log("Включение щита: " + state);
        IsShielded = enable;
        _shieldAnimator.ShieldEnableAnimate(enable, _shield);
        if (enable) {
            _hpView.UsePlayerShield();
        }
    }

    public void SetMaxHp() {
        CurrentHp = MaxHp;
        _hpView.SetMaxHp(CurrentHp, _stayInLeft);
    }

    
    public void SetDead() {
        CurrentHp = 0;
    }


}