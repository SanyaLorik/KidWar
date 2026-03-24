using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class ThrowGameStarter : MonoBehaviour  {
    // Шарит за всю инфу по игре, где кто находится 
    [Header("Время")]
    [SerializeField] private float _timerDuration;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _timerCanvas;

    [Inject] private BattleManager _battleManager;
    [Inject] private LocalizationData _localization;

    private CancellationTokenSource _tokenSource;
    
    public event Action<bool> GameStarted;
    

    private void Start() {
        StartTimer();
    }


    public void GameOver() {
        GameStarted?.Invoke(false);
        // Ну наверное начинать сразу...
        StartTimer();
    }
    
    
    /// <summary>
    /// Кнопка афк
    /// </summary>
    public void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

    private void StartTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        NewGameTimer(_tokenSource.Token).Forget();
    }
    
    private async UniTaskVoid NewGameTimer(CancellationToken token) {
        float elapsedTime = 0f;
        _timerCanvas.ActiveSelf();
        while (elapsedTime <  _timerDuration && !token.IsCancellationRequested) {
            elapsedTime += Time.deltaTime;
            string timerText = string.Format(
                _localization.Timer,
                _localization.GetPrettyTime((int)(_timerDuration - elapsedTime))
            );
            _timerText.text = timerText;
            await UniTask.Yield();
        }
        _timerCanvas.DisactiveSelf();
        if (!token.IsCancellationRequested) {
            StartGame();
        }
    }

    // Пока просто с ботиком 
    private void StartGame() {
        _battleManager.InitForNewGame();
        Debug.Log("Старт игры!");
        GameStarted?.Invoke(true);
    }
}
