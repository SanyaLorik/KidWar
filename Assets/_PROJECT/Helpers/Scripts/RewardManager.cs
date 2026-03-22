using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MirraSDK_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RewardManager : MonoBehaviour {
    // [SerializeField] private GameObject _canvas;
    // [SerializeField] private RectTransform _container;
    //
    //
    // [SerializeField] private Button _backButton;
    // [SerializeField] private Button _backButton2x;
    // [SerializeField] private RectTransform _outScreenCavasPoint; 
    //
    //
    // [SerializeField] private float _canvasShowSpeed;
    // [SerializeField] private float _timeToHideBackButton = 2f;
    // [Header("ЫЗЫ для анимации")]
    // [SerializeField] private Ease _hideCanvasEase;
    // [SerializeField] private Ease _showCanvasEase;
    //
    // [Header("Динамичный текст")]
    // [SerializeField] private TMP_Text _distanceText;
    // [SerializeField] private TMP_Text _rewardText;
    // [SerializeField] private TMP_Text _rewardText2x;
    //
    //
    // private float _yInScreenPosition; 
    // private float _yOutScreenPosition; 
    //
    //
    // private bool _inAnimation => _animation != null && _animation.active;
    // private Sequence _animation;
    // private CancellationTokenSource _tokenSource;
    //
    // [Inject] private PlayerStateManager _playerStateManager;
    // [Inject] private PlayerMovement _playerMovement;
    // [Inject] private ZoneManager _zoneManager;
    //
    //
    // [Inject] private PlayerBank _playerBank;
    // [Inject] private UpgradesCalculator _upgradesCalculator;
    // [Inject] private LocalizationDataPC _localization;
    // [Inject] private NumberFormatter _formatter;
    // [Inject] private AdvertisingMonetizationMirra _advertisingMonetizationMirra;
    //
    //
    //
    // public void OnEnable() {
    //     _playerStateManager.ChangeState += OnStateChange;
    // }
    //
    //
    // private void Start() {
    //     _canvas.ActiveSelf();
    //     _backButton.AddListenerWithSound(() => Reward(false));
    //     _backButton2x.AddListenerWithSound(Reward2x);
    //     _yInScreenPosition = _container.anchoredPosition.y;
    //     _yOutScreenPosition = RectTransformHelper.GetYBottomScreen(_container, _outScreenCavasPoint);
    //     _container.anchoredPosition = new Vector2(_container.anchoredPosition.x, _yOutScreenPosition);
    //     _canvas.DisactiveSelf();
    // }
    //
    // private void OnStateChange(PlayerState state) {
    //     if (state == PlayerState.Cruisered) {
    //         ShowReward(true);
    //     }
    //     else if (state == PlayerState.Grounded) {
    //         ShowReward(false);
    //     }
    // }
    //
    // private double _reward;
    // private bool _isCruisered;
    // private void ShowReward(bool cruisered) {
    //     if (_playerStateManager.BeforeState == PlayerState.Walking) {
    //         _playerMovement.TpPlayerInBetZone();
    //         return;
    //     }
    //
    //     _isCruisered = cruisered;
    //     ShowRewardWindowAnimation();
    //     if (cruisered) {
    //         _reward = _upgradesCalculator.GetWinMoney(); 
    //     }
    //     else {
    //         _reward = _upgradesCalculator.GetDistanceMoney();
    //         
    //     }
    //     ShowBaseRewardVisual(_reward);
    // }
    //
    //
    //
    //
    //
    //
    // private void ShowBaseRewardVisual(double reward) {
    //     // Выигрышь
    //     _distanceText.text = _playerStateManager.CurrentPlayerDistance() + _localization.Meters;
    //     _rewardText.text = _formatter.ValuteFormatter(reward);
    //     _rewardText2x.text = _formatter.ValuteFormatter(reward*2);
    // }
    //
    // private void Reward(bool doubleReward) {
    //     int multiplier = doubleReward ? 2 : 1;
    //     if (!_isCruisered) {
    //         _playerBank.GetSilentBetFallMoney(_zoneManager.BetAmount);
    //     }
    //     _playerBank.AddMoney(_reward * multiplier);
    //     
    //     
    //     _playerMovement.TpPlayerInSpawn();
    //     _playerStateManager.ChangePlayerState(PlayerState.Walking);
    //     Debug.LogWarning("Занос бабок " + _reward * multiplier);
    //     HideRewardWindowAnimation();
    // }
    //
    // private void Reward2x() {
    //     _advertisingMonetizationMirra.InvokeRewarded(
    //         null,
    //         (isSuccess) => 
    //         {
    //             if (isSuccess) {
    //                 Reward(true);
    //             }
    //         }
    //     );
    //    
    // }
    //
    //
    // private void ShowRewardWindowAnimation() {
    //     _canvas.ActiveSelf();
    //     _backButton.DisactiveSelf();
    //     _container
    //         .DOAnchorPosY(_yInScreenPosition, _canvasShowSpeed)
    //         .SetEase(_showCanvasEase)
    //         .OnComplete(ShowWithDelayBackButton);
    // }
    //
    //
    // private void HideRewardWindowAnimation() {
    //     _container.
    //         DOAnchorPosY(_yOutScreenPosition, _canvasShowSpeed)
    //         .SetEase(_hideCanvasEase)
    //         .OnComplete(_canvas.DisactiveSelf);
    //
    // }
    //
    // private void ShowWithDelayBackButton() {
    //     _tokenSource?.Cancel();
    //     _tokenSource = new CancellationTokenSource();
    //     UniTaskHelper.TimerAction(
    //         _timeToHideBackButton,
    //         BackButtonAnimation,
    //         _tokenSource.Token
    //     ).Forget();
    // }
    //
    // private void BackButtonAnimation() {
    //     _backButton.ActiveSelf();
    //     _backButton.transform.DOScale(1, _canvasShowSpeed).From(0).SetEase(Ease.OutBounce);
    // }
    //
    //
    // private void OnDestroy() {
    //     if (_inAnimation) {
    //         _animation.Kill();
    //     }
    // }
}
