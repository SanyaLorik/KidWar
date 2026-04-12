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
    [Inject] private AdvTimerStarter _advTimerStarter;
    [Inject] private TutorialManager _tutorialManager;


    private bool _afkPressed;
    private bool _startGamePressed;
    
    private CancellationTokenSource _tokenSource;
    
    public event Action<bool> GameStarted;
    private bool _firstPlayerBot;
    private bool _secondPlayerBot = true;
    public bool FirstPlayerBot => _firstPlayerBot;

    
    public bool GameIsStarted { get; private set; }
    
    
    private void Start() {
        float timeToStart = _tutorialManager.TutorialPassed ? _timerDuration : 0f;
        StartTimer(timeToStart);
        _afkStatusText.DisactiveSelf();
    }
    
    public void GameOver() {
        Debug.Log("GameOver, _startGamePressed = " + _startGamePressed);
        GameStarted?.Invoke(false);
        GameIsStarted = false;
        if (_battleManager.MainPlayerPlay) {
            _advTimerStarter.ShowAdvAfterBattle();
        }
        
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
        SystemEvents.WindowOpened += EnableAfkWindow;
    }

    private bool _cachedAfkState;
    private void EnableAfkWindow(bool windowOpened) {
        if (windowOpened) {
            _cachedAfkState = _afkPressed;
            ChangeAfkStatus(true, false);
        }
        else {
            ChangeAfkStatus(_cachedAfkState, false);
        }
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
    
    public void ChangeAfkStatus(bool afk, bool changeVisual = true) {
        // Debug.Log("ChangeAfkStatus");
        _afkPressed = afk;
        _firstPlayerBot = _afkPressed;
        _secondPlayerBot = true;
        if (changeVisual) {
            _afkStatusText.SetActive(_afkPressed);
        }
        // Ушел в афк врубаем таймер
        if (afk) {
            _advTimerStarter.EnableTimer();
        }

    }


    private void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _timerCanvas.DisactiveSelf();
    }

    private void StartTimer(float time) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        // Таймер стартанул где игрок будет играть, вырубаем таймер нахуй
        if (!_afkPressed) {
            _advTimerStarter.DisableTimer();
        }
        NewGameTimer(time, _tokenSource.Token).Forget();
    }
    
    
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

    private void StartGame() {
        GameIsStarted = true;
        _battleManager.InitForNewGame(_firstPlayerBot, _secondPlayerBot);
        GameStarted?.Invoke(true);
        _startGamePressed = false;
        // Debug.Log("Старт игры!");
    }
}
