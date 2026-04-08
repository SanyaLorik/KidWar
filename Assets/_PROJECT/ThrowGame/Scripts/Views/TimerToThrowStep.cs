using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class TimerToThrowStep : MonoBehaviour {
    [SerializeField] private RectTransform _progress;
    [SerializeField] private RectTransform _progressParent;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _timer;
    [SerializeField] private int _secondsToThrowStep;
    
    [Inject] TutorialManager _tutorialManager;
    
    public event Action TimeIsOver;
    
    private CancellationTokenSource _tokenSource;

    [Inject] private LocalizationData _localization;
    [Inject] private ThrowGameStarter _gameStarter;

    
    private void OnEnable() {
        _gameStarter.GameStarted += OnGameStarted;
    }

    private void OnGameStarted(bool started) {
        if (!started) {
            StopTimer();
        }
    }

    private void Start() {
        _timer.DisactiveSelf();
    }


    public void StartTimer() {
        if(!_tutorialManager.TutorialPassed) return;
        
        _timer.ActiveSelf();
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartTimerAsync(_tokenSource.Token).Forget();
    }
    

    public void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        SetTimerFullPercentage();
        _timer.DisactiveSelf();
    }


    private void SetTimerFullPercentage() {
        SetFillAmountInLeft(_progress, _progressParent, 1f);
        _timerText.text = _localization.GetPrettyTime(_secondsToThrowStep);
    }

    private async UniTask StartTimerAsync(CancellationToken token) {
        int currentSeconds = _secondsToThrowStep;
        while (!token.IsCancellationRequested && currentSeconds > 0) {
            _timerText.text = _localization.GetPrettyTime(currentSeconds);
            
            float progress = (float) currentSeconds / _secondsToThrowStep;
            SetFillAmountInLeft(_progress, _progressParent, progress);
            
            currentSeconds -= 1;
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
        }
        TimeIsOver?.Invoke();
    }
    
    
    public static void SetFillAmountInLeft(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var a = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        img.offsetMax = a;
    }
    
    private static float GetXPoseByPercent(float percent, float xEnd, RectTransform parent)
    {
        if (xEnd < 0)
        {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }
}
