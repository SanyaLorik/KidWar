using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


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
    [Header("Минимальное время в воздухе")] 
    [SerializeField] private float _minFlightDuration;
    
    [field: Header("Инфа по уровню")]
    [field: SerializeField] public Transform LeftPoint { get; private set; }
    [field: SerializeField] public Transform RightPoint { get; private set; }
    [field: Header("Отступ от игрока при макс броске")]
    [field: SerializeField] public float OffsetToMaxThrow { get; private set; }
    [SerializeField] private Transform _floorPoint;
    
    [Header("Задержка перед броском в секундах")]
    [SerializeField] private float _durationBeforeThrow = .3f;


    
    
    
    private float _initialDistance;
    private CancellationTokenSource _tokenSource;
    private List<ThrowableObject> _throwInstances = new();
    
    
    public event Action<Transform> ObjectThrowed;
    public event Action PlayerPressThrow;
    public event Action ObjectFalled;

    public bool ObjectInFly { get; private set; }


    [Inject] ThrowGameStarter _gameStarter;
    [Inject] WindChooseView _wind;
    [Inject] ForceChooseView _force;
    [Inject] ModifierManager _modifierManager;

    private void OnEnable() {
        _gameStarter.GameStarted += BattleManagerOnGameIsStarted;
    }

    private void BattleManagerOnGameIsStarted(bool isStarted) {
        ObjectInFly = false;
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        WaitFewFramesToDelete(_tokenSource.Token).Forget();
    }

    private async UniTask WaitFewFramesToDelete(CancellationToken token) {
        await UniTask.WaitForSeconds(.5f, cancellationToken: token);
        _throwInstances.ForEach(obj => Destroy(obj.gameObject));
        _throwInstances.Clear();
        
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

    
    private float DistanceBeforePlayers => Vector3.Distance(LeftPoint.position, RightPoint.position);
    
    public void ThrowNewObject(float angle, ThrowableObject obj, Transform throwPoint, Vector3 enemyPoint) {
        float windSign = throwPoint.position.z < enemyPoint.z ? 1 : -1;
        
        
        _throwDistance = CalculateThrowDistance(angle, windSign);
        _height = CalculateHeight(angle);
        _trajectoryLength = CalculateTrajectoryLength(_throwDistance, _height);
        
        _flightDuration = _trajectoryLength / _flySpeed;
        _flightDuration = MathF.Max(_flightDuration, _minFlightDuration);
        
        
        _flightDurationToEnemy = CalculateTrajectoryLength(DistanceBeforePlayers, _height) / _flySpeed;

        
        // Debug.Log("_flightDuration = " + _flightDuration);
        // Debug.Log("_flightDurationToEnemy = " + _flightDurationToEnemy);
        // Debug.Log("Ветер сейчас = " + _wind.CurrentWindForce);
        
        
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

    /// <summary>
    /// Выбирается точка куда бот хочет попасть
    /// </summary>
    /// <param name="throwDistance"> - дистанция от него до точки</param>
    /// <returns></returns>
    public (float force, float angle) CalculateForceAndAngleToPoint(float throwDistance, float throwZ, float enemyZ) {
        // Формула
        // _throwDistance = _initialDistance * _force.CurrentForce * _angleRatio + _wind.CurrentWindForce * windSign;
        // _initialDistance известна
        // _throwDistance известна
        // нам нужно подобрать что-то, допустим бот выбирает еще силу, тогда надо выразить из формулы и получить угол 
        // или если угол выбирает тогда получаем силу
        // _force.CurrentForce * _angleRatio = (_throwDistance - _wind.CurrentWindForce * windSign) / _initialDistance;
        
        
        // _throwDistance = _initialDistance * _force.CurrentForce * _angleRatio + _wind.CurrentWindForce * windSign;
        // _throwDistance - _wind.CurrentWindForce * windSign = _initialDistance * _force.CurrentForce * _angleRatio
        // _force.CurrentForce = (_throwDistance - _wind.CurrentWindForce * windSign) / _angleRatio * _initialDistance;
        
        // Погрешность влево вправо похуй
        // Debug.Log("Ветер сейчас = " + _wind.CurrentWindForce);
        
        // Шаг итерации
        float angleEps = .5f;
        // Начальная сила
        // Чуть заранее от самого сильного 
        float startAngle = Random.Range(20,50);
        float angleRatio = CalculateAngleRatio(startAngle);
        float windSign = throwZ < enemyZ ? 1 : -1;
        // Начинаем наверное с подбора угла
        float currForce = (throwDistance - _wind.CurrentWindForce * windSign) / (angleRatio * _initialDistance);
        if (currForce > 1f) {
            int maxAttempts = 100; // защита от бесконечного цикла
            startAngle = _angleWithMaxDistance; 
            while (currForce > 1f && startAngle >= _angleDiapasone.From && maxAttempts > 0) {
                startAngle -= angleEps;
                angleRatio = CalculateAngleRatio(startAngle);
                currForce = (throwDistance - _wind.CurrentWindForce * windSign) / (angleRatio * _initialDistance);
                maxAttempts--;
            }
        }

        // Debug.Log($"Для дистанции {throwDistance} угол {startAngle}, сила = {currForce}");
        // float distance = _initialDistance * currForce * angleRatio + _wind.CurrentWindForce * windSign;
        // Debug.Log("Ветер сейчас = " + _wind.CurrentWindForce);
        // Debug.Log("Подставив в формулу мы получим distance = " + distance);
        currForce = MathF.Min(currForce, 1f);
        return (currForce, startAngle);
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


        ObjectInFly = true;
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
            _modifierManager.CurrentModifier
        );
        ObjectThrowed?.Invoke(throwInstance.transform);
        await throwInstance.StartFlight(token);
        
        Debug.Log("Обьект упал! " + throwInstance.transform.position);
        throwInstance.ObjectIsFall();
        await UniTask.WaitForSeconds(1f, cancellationToken: token);
        ObjectInFly = false;
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
        // Debug.Log($"distance={distance}, height={height}, length={trajectoryLength}");
    
        return trajectoryLength;
    }
    
    
    private float CalculateAngleRatio(float angle) {
        // Отклонение от 45°, максимальное 45°
        float diff = Mathf.Abs(angle - _angleWithMaxDistance);
    
        // ratio: 1 при 45°, 0 при 0° или 90°
        float ratio = 1f - (diff / _angleWithMaxDistance);
    
        return Mathf.Max(_minAngleRatio, ratio);
    }

}
