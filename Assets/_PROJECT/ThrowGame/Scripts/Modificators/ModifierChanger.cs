using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ModifierChanger : MonoBehaviour {
    [SerializeReference, SubclassSelector] IThrowableModifier _throwableModifier;
    [SerializeField] private GameObject _pointerToModifier;
    [SerializeField] private Image _availableImage;
    [SerializeField] private float _timeToColldown;
    
    
    private bool _isAvailable;
    private CancellationTokenSource _tokenSource;
    
    public IThrowableModifier Modifier => _throwableModifier;
    
    [Inject] ModifierManager _modifierManager;



    public void ChangeModifier() {
        if (!_isAvailable) {
            Debug.Log("Модификатор на перезарядке именно что");
            return;
        }
        _modifierManager.TrySetModifier(_throwableModifier, this);
    }

    public void ShowPointer() {
        _pointerToModifier.ActiveSelf();
    }
    
    public void HidePointer() {
        _pointerToModifier.DisactiveSelf();
    }

    public void SetAvailable() {
        _isAvailable = true;
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _availableImage.fillAmount = 0f;
    }
    
    public void StartColldown() {
        _isAvailable = false;
        _availableImage.fillAmount = 1f;
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
