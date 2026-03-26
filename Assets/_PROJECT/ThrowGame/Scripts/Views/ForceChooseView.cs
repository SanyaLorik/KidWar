using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ForceChooseView : MonoBehaviour {
    [SerializeField] private float _timeToFullCycle;
    
    [SerializeField] private GameObject _canvas;
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private RectTransform _progressParent;
    [Header("Диапазон силы")] 
    [SerializeField] private float _forceMax;
    [SerializeField] private float _minForce;
    [Header("Кривая скорости")] 
    [SerializeField] private AnimationCurve _forwardCurve;  // Кривая для движения 0→0.5
    [SerializeField] private AnimationCurve _backwardCurve; // Кривая для движения 0.5→1
    [Header("Плитки силы")] 
    [SerializeField] private GameObject[] _forceTiles;

    [Inject] private ObjectThrowerCalculator _objectThrowerCalculator;
    [Inject] private ThrowGameStarter _throwGameStarter;
    [Inject] private InputThrowGame _inputThrowGame;
    [Inject] private ThrowGameStarter _gameStarter;
    
    private float _currentForcePercent;
    private CancellationTokenSource _tokenSource;
    private bool _gameIsStarted;


    public float CurrentForce => Mathf.Max(_forceMax * _currentForcePercent, _minForce); 

    private void OnEnable() {
        _objectThrowerCalculator.PlayerPressThrow += StopChooser;
        // Скрою опять мало ли игрок решит еще раз на экран нажать
        _objectThrowerCalculator.ObjectFalled += StopChooser;
        _throwGameStarter.GameStarted += ThrowGameStarterOnGameStarted;
        // Опять же если бот то игрок не сможет скрыть
        _inputThrowGame.OnDowned += ShowForceView;
        _inputThrowGame.OnUpped += HideForceView;
        // Убираем поинтер
        _pointer.DisactiveSelf();
    }

    private void HideForceView() {
        // Проверка что ход не бота еще сделать
        _canvas.DisactiveSelf();
        StopChooser();
    }

    private void ShowForceView() {
        if(_objectThrowerCalculator.ObjectInFly) return;
        _canvas.ActiveSelf();
        StartChooser();
    }

    private void ThrowGameStarterOnGameStarted(bool isStarted) {
        _canvas.DisactiveSelf();
        _gameIsStarted = isStarted;
    }

    private void StopChooser() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    
    private void StartChooser() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartForceChooserAsync(_tokenSource.Token).Forget();
    }

    private async UniTask StartForceChooserAsync(CancellationToken token) 
    {
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested) 
        {
            elapsedTime += Time.deltaTime;
            float pingPongValue = Mathf.PingPong(elapsedTime, _timeToFullCycle) / _timeToFullCycle;
        
            float curvedT;
            // Проверяем направление: возрастает или убывает
            if (pingPongValue < _timeToFullCycle)
            {
                // Идем от 0 к _timeToFullCycle -> _currentForcePercent от 0 к 1
                curvedT = _forwardCurve.Evaluate(pingPongValue);
            }
            else
            {
                // Идем от _timeToFullCycle к 0 -> pingPongValue от 1 к 0
                curvedT = _backwardCurve.Evaluate(1f - pingPongValue);
            }

            _currentForcePercent = curvedT;
        
            UpdateTiles(curvedT);
            await UniTask.Yield();
        }
    }
    
    
    private void UpdateTiles(float forcePercent) 
    {
        // forcePercent от 0 до 1
        // Сколько плиток должно быть активно (0..n)
        int activeCount = Mathf.FloorToInt(forcePercent * _forceTiles.Length);
        activeCount = Mathf.Clamp(activeCount, 0, _forceTiles.Length);
    
        // Включаем/выключаем плитки
        for (int i = 0; i < _forceTiles.Length; i++)
        {
            _forceTiles[i].SetActive(i <= activeCount);
        }
    }
    
    
    // Visual --> Move later to RectTransformHelper
    private static float CalculateYEnd(RectTransform parent) => parent.rect.height;
    
    
    public static void SetPointerYNegative(RectTransform pointer, float percent, float yEnd, float offset = 0) {
        Vector2 newPointerPos = new Vector2(pointer.anchoredPosition.x, yEnd * percent + offset);
        pointer.anchoredPosition = newPointerPos;
    }
}
