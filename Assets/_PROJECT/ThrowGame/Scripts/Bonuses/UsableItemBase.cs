using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class UsableItemBase : MonoBehaviour {
    [SerializeField] private float _timeToColldown;
    [SerializeField] private Image _availableImage;

    private CancellationTokenSource _tokenSource;

    public bool IsAvailable { get; private set; }
    public abstract void TryUse();
    
    public void SetAvailable() {
        IsAvailable = true;
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _availableImage.fillAmount = 0f;
    }

    public void SetUnvailable(bool startCooldown = false) {
        IsAvailable = false;
        _availableImage.fillAmount = 1f;
        if (startCooldown) {
            StartColldown();
        }
    }

    /// <summary>
    /// Чтоб визуально показать игроку что он не может тыкать на модификаторы другого игрока
    /// </summary>
    public void SetVisualGray(bool set) {
        if(!IsAvailable) return;
        _availableImage.fillAmount = set ? 1f : 0f;
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
        IsAvailable = true;
    }
}