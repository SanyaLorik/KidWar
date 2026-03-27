using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class UsableItemBase : MonoBehaviour {
    [SerializeField] private float _timeToColldown;
    [SerializeField] private Image _availableImage;

    private CancellationTokenSource _tokenSource;


    protected bool _isAvailable;
    public abstract void TryUse();
    
    public void SetAvailable() {
        _isAvailable = true;
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _availableImage.fillAmount = 0f;
    }
    
    public void SetUnvailable(bool startCooldown = false) {
        _isAvailable = false;
        _availableImage.fillAmount = 1f;
        if (startCooldown) {
            StartColldown();
        }
    }
    
    private void StartColldown() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        CooldownAsync(_tokenSource.Token).Forget();
    }
    
    private async UniTask CooldownAsync(CancellationToken token) {
        float elapsedTime = 0f;
        while (elapsedTime < _timeToColldown && !token.IsCancellationRequested) {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _timeToColldown;
            _availableImage.fillAmount = 1f - progress;
            await UniTask.Yield();
        }
        _isAvailable = true;
    }
}