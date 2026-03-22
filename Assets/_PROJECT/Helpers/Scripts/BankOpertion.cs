using System.Collections.Generic;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class BankOpertion : MonoBehaviour {
    // [SerializeField] private Transform _parentForMoney;
    // [SerializeField] private float _animationDuration = 1f;
    // [SerializeField] private float _upOffset = 200f;
    // [SerializeField] private NewMoneyView _newMoneyView;
    // [SerializeField] private Transform _playerSpawnPoint;
    // [SerializeField] private float _spawnRadius;
    // [SerializeField] private float _trampolineMultiplierRadius;
    //
    //
    // private readonly Queue<NewMoneyView> _bankOpertaionViewPool = new ();
    // private float _lastAngle;
    // private readonly float _angleStep = 70f; // шаг в градусах
    //
    // [Inject] private PlayerBank _bank;
    // [Inject] private NumberFormatter _formatter;
    // [Inject] private PlayerStateManager _playerStateManager;
    //
    // private void OnEnable() {
    //     _bank.BankNewMoneyPlus += BankPlus;
    //     _bank.BankNewMoneyMinus += BankMinus;
    // }  
    //
    // private void BankPlus(long money) {
    //     NewMoneyView newMoneyView = GetBankOperationViewInPool();
    //     newMoneyView.transform.position = GetPointAroundPlayerSpiral();;
    //     newMoneyView.PlusMoney(_formatter.ValuteFormatter(money));
    //     HideBankOpertionViewAnimation(newMoneyView);
    // }
    //
    // private void BankMinus(long money) {
    //     if(money == 0) return;
    //     NewMoneyView newMoneyView = GetBankOperationViewInPool();
    //     newMoneyView.transform.position = GetPointAroundPlayerSpiral();;
    //     newMoneyView.MinusMoney(_formatter.ValuteFormatter(money));
    //     HideBankOpertionViewAnimation(newMoneyView);
    // }
    //
    //
    // private NewMoneyView GetBankOperationViewInPool() {
    //     if (_bankOpertaionViewPool.Count > 0)
    //         return _bankOpertaionViewPool.Dequeue();
    //     return Instantiate(_newMoneyView, _parentForMoney);
    // }
    //
    // private void HideBankOpertionViewAnimation(NewMoneyView newMoneyView) {
    //     newMoneyView.ActiveSelf();
    //     
    //     
    //     float targetY = newMoneyView.transform.position.y + _upOffset;
    //     Sequence sequence = DOTween.Sequence();
    //
    //     sequence
    //         .Append(newMoneyView.RectTransform.DOAnchorPosY(targetY, _animationDuration))
    //         .Join(newMoneyView.Container.DOFade(0f, _animationDuration))
    //         .OnComplete(() => BankOperationReset(newMoneyView));
    // }
    //
    // private void BankOperationReset(NewMoneyView newMoneyView) {
    //     newMoneyView.Container.alpha = 1f;
    //     newMoneyView.DisactiveSelf();
    //     _bankOpertaionViewPool.Enqueue(newMoneyView);
    //     
    // }
    //
    //
    // private Vector2 GetPointAroundPlayerSpiral() {
    //     float radius = _playerStateManager.CurrentState == PlayerState.TrampolineJumping
    //         ? _spawnRadius * _trampolineMultiplierRadius
    //         : _spawnRadius;
    //
    //     // Увеличиваем угол с каждым спавном
    //     _lastAngle += _angleStep;
    //     if (_lastAngle >= 360f) _lastAngle -= 360f;
    //
    //     float angle = _lastAngle * Mathf.Deg2Rad;
    //     float r = radius * 0.8f; // не используем весь радиус, чтобы было предсказуемо
    //
    //     float x = Mathf.Cos(angle) * r;
    //     float y = Mathf.Sin(angle) * r;
    //     Vector3 screenPos = Camera.main.WorldToScreenPoint(_playerSpawnPoint.transform.position);
    //     Vector2 point = new Vector2(x + screenPos.x, y + screenPos.y);
    //
    //     float padding = 100f;
    //
    //     point = RectTransformHelper.ClampByScreenVector(padding, point);
    //     return point;
    // }
    
    
   
}
