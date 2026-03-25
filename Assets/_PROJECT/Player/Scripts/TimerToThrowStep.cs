using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class TimerToThrowStep : MonoBehaviour {
    [SerializeField] private RectTransform _progress;
    [SerializeField] private RectTransform _progressParent;
    [SerializeField] private TextMeshProUGUI _timerText;
    
    
    public event Action TimeIsOver;
    
    private CancellationTokenSource _tokenSource;

    [Inject] private LocalizationData _localization;
    
    public void StartTimer(int time) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartTimerAsync(time, _tokenSource.Token).Forget();
    }
    

    public void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    

    private async UniTask StartTimerAsync(int time, CancellationToken token) {
        int currentSeconds = time;
        while (!token.IsCancellationRequested && currentSeconds > 0) {
            _timerText.text = _localization.GetPrettyTime(currentSeconds);
            
            float progress = (float) currentSeconds / time;
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
        Debug.Log("img.offsetMax = " + a);
        img.offsetMax = a;
    }
    
    public static void SetFillAmountInRight(RectTransform img, RectTransform parent, float percent)
    {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        var a = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        Debug.Log("img.offsetMin = " + a);
        img.offsetMin = -a;
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
