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
    
    public void TakeDamage(int hp) {
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
        ChangeHpView();
    }


    public void AddHp(int hp) {
        CurrentHp += hp;
        Debug.Log($"Добавление хп {hp} хп, щас HP = {CurrentHp} ");
        CurrentHp = Mathf.Min(CurrentHp, MaxHp);
        ChangeHpView();
    }

    public void SetInvinsible(bool state) {
        IsInvinsible = state;
    }

    public void SetShielded(bool state) {
        Debug.Log("Включение щита: " + state);
        IsShielded = state;
        _shieldAnimator.ShieldEnableAnimate(state, _shield);
    }

    

    public void SetMaxHp() {
        CurrentHp = MaxHp;
        ChangeHpView();
    }
    
    private void ChangeHpView() {
        _hpView.ChangeHp(CurrentHp, _stayInLeft);
    }
    
    public void SetDead() {
        CurrentHp = 0;
    }


}