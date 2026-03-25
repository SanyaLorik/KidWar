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
    [Header("Кривая скорости")] 
    [SerializeField] private AnimationCurve _forwardCurve;  // Кривая для движения 0→0.5
    [SerializeField] private AnimationCurve _backwardCurve; // Кривая для движения 0.5→1

    [Inject] private ObjectThrowerCalculator _objectThrowerCalculator;
    [Inject] private ThrowGameStarter _throwGameStarter;
    private float _currentForcePercent;


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

    private async UniTask StartForceChooserAsync(CancellationToken token) 
    {
        float yEnd = CalculateYEnd(_progressParent);
        SetPointerYNegative(_pointer, 0f, yEnd);
    
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested) 
        {
            elapsedTime += Time.deltaTime;
        
            // Получаем линейное PingPong от 0 до 1
            float linearT = Mathf.PingPong(elapsedTime, _timeToFullCycle) / _timeToFullCycle;
        
            // Применяем разные кривые в зависимости от фазы
            float curvedT;
            if (linearT <= 0.5f)
            {
                // Первая половина: идем от 0 к 1
                float t = linearT / 0.5f; // Нормализуем в 0→1
                curvedT = _forwardCurve.Evaluate(t) * 0.5f; // Масштабируем в 0→0.5
            }
            else
            {
                // Вторая половина: идем от 1 к 0
                float t = (linearT - 0.5f) / 0.5f; // Нормализуем в 0→1
                curvedT = 0.5f + _backwardCurve.Evaluate(t) * 0.5f; // Масштабируем в 0.5→1
            }
        
            _currentForcePercent = curvedT;
        
            // Сведём до [-1:1]
            float percentToVisual = -1f + _currentForcePercent * 2;
        
            // деление на 2f для пивота
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
