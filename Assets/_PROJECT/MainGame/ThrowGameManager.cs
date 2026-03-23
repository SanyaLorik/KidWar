using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class ThrowGameManager : MonoBehaviour  {
    // Шарит за всю инфу по игре, где кто находится 
    [Header("Время")]
    [SerializeField] private float _timerDuration;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Inject] private PlayersIniter _gameIniter;
    [Inject] private LocalizationData _localization;

    private CancellationTokenSource _tokenSource;
    
    private void Start() {
        StartTimer();
    }

    
    private void StartTimer() {
       UniTaskHelper.DisposeTask(ref _tokenSource);
       _tokenSource = new CancellationTokenSource();
       NewGameTimer(_tokenSource.Token).Forget();
    }
    
    
    private void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

    private async UniTaskVoid NewGameTimer(CancellationToken token) {
        float elapsedTime = 0f;
        while (elapsedTime <  _timerDuration && !token.IsCancellationRequested) {
            elapsedTime += Time.deltaTime;
            _timerText.text = _localization.GetPrettyTime((int)(_timerDuration - elapsedTime));
            await UniTask.Yield();
        }

        StartGame();
    }

    // Пока просто с ботиком 
    private void StartGame() {
        _gameIniter.InitForNewGame();
        Debug.Log("Старт игры!");
    }
}
