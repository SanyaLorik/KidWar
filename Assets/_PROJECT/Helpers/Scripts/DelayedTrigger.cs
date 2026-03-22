using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;


public class DelayedTrigger : MonoBehaviour {
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Image _progress;
    [SerializeField] private Color _notAvailableColor;
    [SerializeField] private Color _availableColor;
    
    private CancellationTokenSource _tokenSource;


    public void DelayedTriggerAction(Action action) {
        _tokenSource?.Cancel();
        _tokenSource = new CancellationTokenSource();
        ProgressVisual(_tokenSource.Token, action).Forget();
    }

    public void CancelTriggerAction() {
        _tokenSource?.Cancel();
        _progress.fillAmount = 1f;
    }
    
    public void SetUnvailable() {
        if (_progress.color != _notAvailableColor) {
            _progress.color = _notAvailableColor;
        }
    }

    public void SetAvailable() {
        if (_progress.color != _availableColor) {
            _progress.color = _availableColor;
        }
    }

    private async UniTask ProgressVisual(CancellationToken token, Action action) {
        float elapsedTime = 0f;
        _progress.fillAmount = 0f;
        while (!token.IsCancellationRequested && elapsedTime < _duration) {
            elapsedTime += Time.deltaTime;
            _progress.fillAmount = Mathf.Clamp01(elapsedTime / _duration);
            await UniTask.Yield();
        }
        _progress.fillAmount = 1f;
        if (!token.IsCancellationRequested) {
            action?.Invoke();
        }

    }


    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }
}
