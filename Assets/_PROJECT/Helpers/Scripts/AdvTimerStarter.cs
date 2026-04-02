using System.Collections;
using Architecture_M;
using UnityEngine;
using Zenject;

public class AdvTimerStarter : MonoBehaviour {
    // [Inject] private TutorialCompiller _tutorialCompiller;
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private BattleManager  _battleManager;

    
    private Coroutine _timerCoroutine;
    public void EnableTimer() {
        if (_timerCoroutine != null) {
            StopCoroutine(_timerCoroutine);
        }
        _timerCoroutine =  StartCoroutine(EnableTimerAsync());
    }

    public void DisableTimer() {
        if (_timerCoroutine != null) {
            StopCoroutine(_timerCoroutine);
        }
        _interstitialDelaying.DisableTimer();

    }
    
    private IEnumerator EnableTimerAsync() {
        yield return new WaitForSeconds(3f);
        if (!_battleManager.MainPlayerPlay) {
            _interstitialDelaying.EnableTimer();
        }
    }
}