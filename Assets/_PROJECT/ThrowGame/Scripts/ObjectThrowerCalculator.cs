using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


/// <summary>
/// Базовый класс для кидателя обьектов
/// </summary>
public class ObjectThrowerCalculator : MonoBehaviour {
    [Header("Анимационная кривая")]
    [SerializeField] private AnimationCurve _throwCurve;
    
    [Header("Диапазон угла бросания")]
    [SerializeField] private PairedValue<float> _angleDiapasone;
    [Header("Угол при котором будет максимальный бросок")]
    [SerializeField] private float _angleWithMaxDistance;
    [Header("Коэффициенты меньшего и большего угла")]
    [SerializeField] private float _minAngleRatio;
    
    [Header("Базовая высота")]
    [SerializeField] private float _baseHeight = 10f;
    [Header("Высота если под 0 градусов")]
    [SerializeField] private float _minTrajectoryLength;
    
    [Header("Скорость полёта")] 
    [SerializeField] private float _flySpeed;
    
    [field: Header("Инфа по уровню")]
    [field: SerializeField] public Transform LeftPoint { get; private set; }
    [field: SerializeField] public Transform RightPoint { get; private set; }
    [field: Header("Отступ от игрока при макс броске")]
    [field: SerializeField] public float OffsetToMaxThrow { get; private set; }
    [SerializeField] private Transform _floorPoint;
    
    [Header("Задержка перед броском в секундах")]
    [SerializeField] private float _durationBeforeThrow = .3f;

    
    // По умолчанию будет обычный полёт
    private ThrowableModifierDefault _defaultModifier;
    private IThrowableModifier _currentModifier;
    
    
    
    private float _initialDistance;
    private CancellationTokenSource _tokenSource;
    private List<ThrowableObject> _throwInstances = new();
    
    
    public event Action<Transform> ObjectThrowed;
    public event Action PlayerPressThrow;
    public event Action ObjectFalled;
    

    
    [Inject] ThrowGameStarter _gameStarter;
    [Inject] WindChooseView _wind;
    [Inject] ForceChooseView _force;

    private void OnEnable() {
        _gameStarter.GameStarted += BattleManagerOnGameIsStarted;
        _currentModifier = _defaultModifier;
    }

    private void BattleManagerOnGameIsStarted(bool isStarted) {
        if (isStarted) {
            _throwInstances.ForEach(obj => Destroy(obj.gameObject));
            _throwInstances.Clear();
        }
    }

    private void Start() {
        CalculateInitialDistance();
    }
    
    private float _throwDistance;
    private float _angleRatio;
    private float _height;
    private float _trajectoryLength;
    private float _flightDuration;
    private float _flightDurationToEnemy;

    public void SetModifier(IThrowableModifier modifier) {
        _currentModifier = modifier;
        Debug.Log("установка модификатора " + _currentModifier.GetType());
    }
    
    private float DistanceBeforePlayers => Vector3.Distance(LeftPoint.position, RightPoint.position);
    
    public void ThrowNewObject(float angle, ThrowableObject obj, Transform throwPoint, Vector3 enemyPoint) {
        float windSign = throwPoint.position.z < enemyPoint.z ? 1 : -1;
        
        
        _throwDistance = CalculateThrowDistance(angle, windSign);
        _height = CalculateHeight(angle);
        _trajectoryLength = CalculateTrajectoryLength(_throwDistance, _height);
        
        _flightDuration = _trajectoryLength / _flySpeed;
        _flightDurationToEnemy = CalculateTrajectoryLength(DistanceBeforePlayers, _height) / _flySpeed;
        Debug.Log("_flightDuration = " + _flightDuration);
        Debug.Log("_flightDurationToEnemy = " + _flightDurationToEnemy);
        
        
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ThrowObject(obj, throwPoint, enemyPoint, _tokenSource.Token).Forget();
    }

    private float CalculateThrowDistance(float angle, float windSign) {
        _angleRatio = CalculateAngleRatio(angle);
        _throwDistance = _initialDistance * _force.CurrentForce * _angleRatio + _wind.CurrentWindForce * windSign;
        Debug.Log("distance " + _throwDistance);
        return _throwDistance;
    }


    private float CalculateHeight(float angle) {
        return _baseHeight * angle / _angleDiapasone.To;
    }
    
    private async UniTask ThrowObject(
        ThrowableObject throwObject, 
        Transform playerThrowPoint, 
        Vector3 enemyPoint, 
        CancellationToken token
    ) {
        // задержка перед броском, можна для анимации
        PlayerPressThrow?.Invoke();
        await UniTask.WaitForSeconds(_durationBeforeThrow, cancellationToken: token);
        
        
        Vector3 initialPos = playerThrowPoint.position;
        Vector3 targetPos = CalculateTargetPose(playerThrowPoint, enemyPoint);


        Debug.Log("Бросок!");
        ThrowableObject throwInstance = Instantiate(throwObject);
        throwInstance.transform.position = initialPos;
        _throwInstances.Add(throwInstance);
        
        throwInstance.SetupAndLaunch(
            initialPos,  
            targetPos,
            enemyPoint,
            _flightDuration, 
            _flightDurationToEnemy,
            _height,
            _throwCurve,
            _currentModifier
        );
        ObjectThrowed?.Invoke(throwInstance.transform);
        await throwInstance.StartFlight(token);
        _currentModifier = _defaultModifier;
        
        Debug.Log("Обьект упал! " + throwInstance.transform.position);
        throwInstance.ObjectIsFall();
        await UniTask.WaitForSeconds(1f, cancellationToken: token);
        ObjectFalled?.Invoke();
    }

    private Vector3 CalculateTargetPose(Transform playerThrowPoint, Vector3 enemyPoint) {
        Vector3 targetPos = playerThrowPoint.position; // начало в точке игрока
        // Логика направления
        float sign = playerThrowPoint.position.z < enemyPoint.z ? 1 : -1;
        targetPos.z += _throwDistance * sign;
        targetPos.y = _floorPoint.position.y;
        return targetPos;
    }

    private void CalculateInitialDistance() {
        _initialDistance = Vector3.Distance(LeftPoint.position, RightPoint.position) + OffsetToMaxThrow;
        Debug.Log("_initialDistance = " + _initialDistance);
    }
    

    private float CalculateTrajectoryLength(float distance, float height) 
    {
        // Если высота близка к нулю (прямая линия)
        if (Mathf.Approximately(height, 0f) || height < 0.01f)
        {
            // Длина дуги = просто расстояние по прямой
            return Mathf.Max(distance, _minTrajectoryLength);
        }
    
        // Аппроксимация длины дуги параболы
        float a = distance / 2f;
        float b = height;
    
        // Формула длины дуги параболы (приближенная)
        float term1 = Mathf.Sqrt(a * a + 4 * b * b);
        float term2 = (a * a) / (2 * b) * Mathf.Log((2 * b + term1) / a);
    
        float trajectoryLength = Mathf.Max((term1 + term2), _minTrajectoryLength);
        Debug.Log($"distance={distance}, height={height}, length={trajectoryLength}");
    
        return trajectoryLength;
    }
    

    private float CalculateAngleRatio(float angle) {
        float diff = Mathf.Abs(angle - _angleWithMaxDistance);
        diff = Mathf.Clamp(diff, _angleDiapasone.From, _angleWithMaxDistance);
        
        float ratio = 1f - Mathf.Clamp01(diff / _angleWithMaxDistance);
        ratio = Mathf.Max(_minAngleRatio, ratio);
        // Debug.Log($"Угол {angle}, ratio: {ratio}");
        return ratio;
    }
}
