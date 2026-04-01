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
    [SerializeField] private float _durationAfterGameOver;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _timerCanvas;
    [SerializeField] private Button _afkButton;
    [SerializeField] private GameObject _afkStatusText;
    [SerializeField] private Button _onlineButton;
    [SerializeField] private Button _duoButton;

    [Inject] private BattleManager _battleManager;
    [Inject] private LocalizationData _localization;

    private bool _afkPressed;
    private bool _startGamePressed;
    
    private CancellationTokenSource _tokenSource;
    
    public event Action<bool> GameStarted;
    private bool _firstPlayerBot;
    private bool _secondPlayerBot = true;

    public bool GameIsStarted { get; private set; }
    
    
    private void Start() {
        _firstPlayerBot = false;
        _secondPlayerBot = true;
        StartTimer(_timerDuration);
        _afkStatusText.DisactiveSelf();
    }
    
    public void GameOver() {
        Debug.Log("GameOver, _startGamePressed = " + _startGamePressed);
        GameStarted?.Invoke(false);
        GameIsStarted = false;
        // Ну наверное начинать сразу...
        if (!_startGamePressed) {
            StartWaitBeforeNewTimer();
        }
    }

    private void StartWaitBeforeNewTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        UniTaskHelper.TimerAction(
            _durationAfterGameOver,
            () => StartTimer(_timerDuration),
            _tokenSource.Token
        ).Forget();
    }


    private void OnEnable() {
        _afkButton.onClick.AddListener(() => ChangeAfkStatus(!_afkPressed));
        _duoButton.onClick.AddListener(StartDuoGame);
        _onlineButton.onClick.AddListener(StartOnlineGame);
    }

    private void StartDuoGame() {
        ChangeAfkStatus(false);
        _startGamePressed = true;
        Debug.Log("StartDuoGame");
        StopTimer();
        _battleManager.SetGameOver();
        _firstPlayerBot = false;
        _secondPlayerBot = false;
        StartTimer(.1f);
    }
    
    public void StartOnlineGame() {
        ChangeAfkStatus(false);
        _startGamePressed = true;
        Debug.Log("StartOnlineGame");
        StopTimer();
        _battleManager.SetGameOver();
        _firstPlayerBot = false;
        _secondPlayerBot = true;
        StartTimer(.1f);
    }
    
    public void ChangeAfkStatus(bool state) {
        Debug.Log("ChangeAfkStatus");
        _afkPressed = state;
        _firstPlayerBot = _afkPressed;
        _secondPlayerBot = true;
        _afkStatusText.SetActive(_afkPressed);
    }
    

    private void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _timerCanvas.DisactiveSelf();
    }

    private void StartTimer(float time) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        NewGameTimer(time, _tokenSource.Token).Forget();
    }
    
    
    // По таймеру играем PVB
    private async UniTaskVoid NewGameTimer(float time, CancellationToken token) {
        Debug.Log("NewGameTimer");
        float elapsedTime = 0f;
        _timerCanvas.ActiveSelf();
        while (elapsedTime <  time && !token.IsCancellationRequested) {
            elapsedTime += Time.deltaTime;
            string timerText = string.Format(
                _localization.Timer,
                _localization.GetPrettyTime((int)(time - elapsedTime))
            );
            _timerText.text = timerText;
            await UniTask.Yield();
        }
        await UniTask.Yield();
        _timerCanvas.DisactiveSelf();
        if (!token.IsCancellationRequested) {
            StartGame();
        }
    }

    // Пока просто с ботиком 
    private void StartGame() {
        GameIsStarted = true;
        _battleManager.InitForNewGame(_firstPlayerBot, _secondPlayerBot);
        GameStarted?.Invoke(true);
        _startGamePressed = false;
        Debug.Log("Старт игры!");
    }
}
