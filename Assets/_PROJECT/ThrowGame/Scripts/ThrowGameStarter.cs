using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ThrowGameStarter : MonoBehaviour  {
    // Шарит за всю инфу по игре, где кто находится 
    [Header("Время")]
    [SerializeField] private float _timerDuration;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _timerCanvas;
    [SerializeField] private Button _afkButton;
    [SerializeField] private GameObject _afkStatusText;
    [SerializeField] private Button _onlineButton;
    [SerializeField] private Button _duoButton;

    [Inject] private BattleManager _battleManager;
    [Inject] private LocalizationData _localization;

    private bool _afkPressed;
    private CancellationTokenSource _tokenSource;
    
    public event Action<bool> GameStarted;
    

    private void Start() {
        StartTimer();
        _afkStatusText.DisactiveSelf();
    }
    
    public void GameOver() {
        Debug.Log("GameOver");
        GameStarted?.Invoke(false);
        // Ну наверное начинать сразу...
        StartTimer();
    }
    

    private void OnEnable() {
        _afkButton.onClick.AddListener(ChangeAfkStatus);
        _duoButton.onClick.AddListener(StartDuoGame);
        _onlineButton.onClick.AddListener(StartOnlineGame);
    }

    private void StartDuoGame() {
        Debug.Log("StartDuoGame");
        StartGame(false, false);
    }
    
    
    private void StartOnlineGame() {
        Debug.Log("StartOnlineGame");
        StartGame(false, true);
    }

    private void ChangeAfkStatus() {
        Debug.Log("ChangeAfkStatus");
        _afkPressed = !_afkPressed;
        _afkStatusText.SetActive(_afkPressed);
    }
    

    private void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _timerCanvas.DisactiveSelf();
    }

    private void StartTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        NewGameTimer(_tokenSource.Token).Forget();
    }
    
    
    // По таймеру играем PVB
    private async UniTaskVoid NewGameTimer(CancellationToken token) {
        Debug.Log("NewGameTimer");
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
            StartGame(_afkPressed, true);
        }
    }

    // Пока просто с ботиком 
    private void StartGame(bool firstPlayerBot, bool secondPlayerBot) {
        _battleManager.InitForNewGame(firstPlayerBot, secondPlayerBot);
        Debug.Log("Старт игры!");
        GameStarted?.Invoke(true);
    }
}
