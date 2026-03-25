using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ForceChooseView : MonoBehaviour {
    [SerializeField] private float _timeToFullCycle;
    
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private RectTransform _progressParent;
    [Header("Диапазон силы")] 
    [SerializeField] private float _forceMax;
    [SerializeField] private float _minForce;

    [Inject] private ObjectThrowerCalculator _objectThrowerCalculator;
    [Inject] private ThrowGameStarter _throwGameStarter;


    public float CurrentForce => Mathf.Max(_forceMax * _currentForcePercent, _minForce); 

    private void OnEnable() {
        _objectThrowerCalculator.PlayerPressThrow += StopChooser;
        _objectThrowerCalculator.ObjectFalled += StartChooser;
        _throwGameStarter.GameStarted += ThrowGameStarterOnGameStarted;
    }

    private void ThrowGameStarterOnGameStarted(bool isStarted) {
        if (isStarted) {
            StartChooser();
        }
    }

    private void StopChooser() {
        Debug.Log("Force choose " + CurrentForce);
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }

    
    private CancellationTokenSource _tokenSource;
    private void StartChooser() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartForceChooserAsync(_tokenSource.Token).Forget();
    }

    private float _currentForcePercent;
    private async UniTask StartForceChooserAsync(CancellationToken token) {
        float yEnd = CalculateYEnd(_progressParent);
        SetPointerYNegative(_pointer, 0f, yEnd);
        
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested) {
            elapsedTime += Time.deltaTime;
            _currentForcePercent = Mathf.PingPong(elapsedTime, 1);
            
            // Сведём до [-1:1] (min + t * distance)
            float percentToVisual = -1f + _currentForcePercent * 2;
            
            // деление на 2f для пивота т.к он от центра в обе стороны на [-0.5:0.5] от высоты
            float percent = percentToVisual / 2f; 
            SetPointerYNegative(_pointer, percent, yEnd);
            await UniTask.Yield();
        }
    }
    
    
    
    // Visual --> Move later to RectTransformHelper
    private static float CalculateYEnd(RectTransform parent) => parent.rect.height;
    
    
    public static void SetPointerYNegative(RectTransform pointer, float percent, float yEnd, float offset = 0) {
        Vector2 newPointerPos = new Vector2(pointer.anchoredPosition.x, yEnd * percent + offset);
        pointer.anchoredPosition = newPointerPos;
    }
}
