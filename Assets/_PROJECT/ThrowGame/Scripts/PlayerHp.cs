using System;
using UnityEngine;
using Zenject;

public class PlayerHp : MonoBehaviour, IDamageable {
    private bool _stayInLeft;
    public int MaxHp => _data.PlayerMaxHp;
    public int CurrentHp { get; private set; }
    public bool IsInvinsible { get; private set; }
    
    
    [Inject] private HpView _hpView;
    [Inject] private GameData _data;
    
    
    public void InitPosition(bool stayInLeft) {
        _stayInLeft = stayInLeft;
    }
    
    public void TakeDamage(int hp) {
        if (IsInvinsible) {
            Debug.Log("Сам по себе попал...");
            return;
        }
        CurrentHp -= hp;
        CurrentHp = Mathf.Max(0, CurrentHp);
        Debug.Log($"Попал! Минус {hp} хп, щас HP = {CurrentHp} ");
        ChangeHpView();
    }


    public void AddHp(int hp) {
        CurrentHp += hp;
        Debug.Log($"Добавление хп {hp} хп, щас HP = {CurrentHp} ");
        CurrentHp = Math.Min(CurrentHp, MaxHp);
        ChangeHpView();
    }

    public void SetInvinsible(bool state) {
        IsInvinsible = state;
    }

    public void SetMaxHp() {
        CurrentHp = MaxHp;
        ChangeHpView();
    }
    
    private void ChangeHpView() {
        _hpView.ChangeHp(CurrentHp, _stayInLeft);
    }


}