using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Architecture_M;
using Cysharp.Threading.Tasks;
using MirraSDK_M;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdvHelper : MonoBehaviour {
    [SerializeField] private float _timeToWait = 3f;

    private CancellationTokenSource _tokenSource;
    private bool _allowToShowAdv;
    
    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private AdvertisingMonetizationMirra _adv;
    [Inject] private BattleManager  _battleManager;
    [Inject] private ThrowGameStarter _mainGameStarter;
    [Inject] private TutorialManager _tutorialManager;
    [Inject] private GameData _gameData;
    
    // Эту ваще офаем
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    // Эту вызываем после двух событий: открытие бонусов и нажатие на кнопку продолжить
    [Inject] private IAdvertisingMonetization _advertisingMonetization;
    

    private void OnEnable() {
        SystemEvents.WindowOpened += OnWindowOpened;
    }
    
    private void OnDisable() {
        SystemEvents.WindowOpened -= OnWindowOpened;
    }

    
    private void Start() {
        _interstitialDelaying.DisableTimer();
        StartAdvTimerAsync().Forget();
    }

    
    public void ShowAdv() {
        if (_allowToShowAdv) {
            _advertisingMonetization.InvokeInterstitial(null, null);
            StartAdvTimerAsync().Forget();
        }
    }
    

    private async UniTask StartAdvTimerAsync() {
        _allowToShowAdv = false;
        await UniTask.WaitForSeconds(_gameData.InterstitialDelay);
        _allowToShowAdv = true;
    }
    
    
    public void EnableTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        EnableTimerAsync(_tokenSource.Token).Forget();
    }

    public void DisableTimer() {
        // UniTaskHelper.DisposeTask(ref _tokenSource);
        // _interstitialDelaying.DisableTimer();
        // Debug.Log("DisableTimer");
    }
    
    
    public void ShowAdvAfterBattle() {
        // ShowAdvAfterBattleAsync().Forget();
    }


    public void AddToButtonAdvRewardListener(Button button, Action callback) {
        button.onClick.AddListener(() => ShowReward(callback));
    }

    
    private void OnWindowOpened(bool open) {
        if (open) {
            DisableTimer();
        }
        else {
            EnableTimer();
        }
    }
    
    
    private void ShowReward(Action callback) {
        _adv.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    callback?.Invoke();
                }
            }
        );
    }


    private async UniTask EnableTimerAsync(CancellationToken token) {
        // await UniTask.WaitForSeconds(_timeToWait, cancellationToken: token);
        // // Условие на непоказ рекламы например если играет или если туториал
        // if (_mainGameStarter.FirstPlayerBot 
        //     && _tutorialManager.TutorialPassed 
        //     && !SystemEvents.WindowIsOpen) 
        // {
        //     _interstitialDelaying.EnableTimer();
        //     Debug.Log("EnableTimer");
        // }
    }


    
    
    private async UniTask ShowAdvAfterBattleAsync() {
        // await UniTask.WaitForSeconds(1);
        // Debug.Log("EnableTimer in ShowAdvAfterBattleAsync");
        // _interstitialDelaying.EnableTimer();
    }
}