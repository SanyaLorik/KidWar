using System.Collections;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AdvTimerStarter : MonoBehaviour {
    [SerializeField] private float _timeToWait = 3f;

    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private BattleManager  _battleManager;
    [Inject] private ThrowGameStarter  _throwGameStarter;
    [Inject] private TutorialManager _tutorialManager;

    private void OnEnable() {
        SystemEvents.WindowOpened += OnWindowOpened;
    }
    
    private void OnDisable() {
        SystemEvents.WindowOpened -= OnWindowOpened;
    }

    private void OnWindowOpened(bool open) {
        if (open) {
            DisableTimer();
        }
        else {
            EnableTimer();
        }
    }

    private CancellationTokenSource _tokenSource;
    public void EnableTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        EnableTimerAsync(_tokenSource.Token).Forget();
    }

    public void DisableTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _interstitialDelaying.DisableTimer();
        Debug.Log("DisableTimer");
    }
    
    private async UniTask EnableTimerAsync(CancellationToken token) {
        await UniTask.WaitForSeconds(_timeToWait, cancellationToken: token);
        // Условие на непоказ рекламы например если играет или если туториал
        if (_throwGameStarter.FirstPlayerBot 
            && _tutorialManager.TutorialPassed 
            && !SystemEvents.WindowIsOpen) 
        {
            _interstitialDelaying.EnableTimer();
            Debug.Log("EnableTimer");
        }
    }

    public void ShowAdvAfterBattle() {
        ShowAdvAfterBattleAsync().Forget();
    }
    
    
    private async UniTask ShowAdvAfterBattleAsync() {
        await UniTask.WaitForSeconds(1);
        Debug.Log("EnableTimer in ShowAdvAfterBattleAsync");
        _interstitialDelaying.EnableTimer();
    }
}