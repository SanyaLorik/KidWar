using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public enum ThrowPositionType {
    Left,
    Right
}


/// <summary>
/// Базовый класс для кидателя обьектов
/// </summary>
public class ObjectThrowerCalculator : MonoBehaviour {
    [Header("Анимационная кривая")]
    [SerializeField] private AnimationCurve _throwCurve;
    
    [Header("Ветра")] 
    [Range(-2,2), SerializeField] private float _windForce;
    
    [Header("Сила броска")] 
    [Range(0,1), SerializeField] private float _throwForce;
    
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
    
    
    
    private float _initialDistance;
    private CancellationTokenSource _tokenSource;
    
    
    public event Action<Transform> ObjectThrowed;
    public event Action ObjectFalled;
    

    private void Start() {
        CalculateInitialDistance();
    }
    
    private float _throwDistance;
    private float _angleRatio;
    private float _height;
    private float _trajectoryLength;
    private float _flightDuration;
    
    public void ThrowNewObject(float angle, ThrowableObject obj, Transform throwPoint, Transform enemyPoint) {
        _throwDistance = CalculateThrowDistance(angle);
        _height = CalculateHeight(angle);
        _trajectoryLength = CalculateTrajectoryLength(_throwDistance, _height);
        _flightDuration = _trajectoryLength / _flySpeed;
        
        
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ThrowObject(obj, throwPoint, enemyPoint, _tokenSource.Token).Forget();
    }

    private float CalculateThrowDistance(float angle) {
        _angleRatio = CalculateAngleRatio(angle);
        _throwDistance = _initialDistance * _throwForce * _angleRatio + _windForce;
        Debug.Log("distance " + _throwDistance);
        return _throwDistance;
    }

    private float CalculateHeight(float angle) {
        return _baseHeight * angle / _angleDiapasone.To;
    }
    
    private async UniTask ThrowObject(
        ThrowableObject throwObject, 
        Transform playerThrowPoint, 
        Transform enemyPoint, 
        CancellationToken token
    ) {
        // задержка перед броском, можна для анимации
        await UniTask.WaitForSeconds(_durationBeforeThrow, cancellationToken: token);
        ObjectThrowed?.Invoke(throwObject.transform);
        
        
        Vector3 initialPos = playerThrowPoint.position;
        Vector3 targetPos = initialPos; // начало в точке игрока
        
        // Логика направления
        float sign = playerThrowPoint.position.z < enemyPoint.position.z ? 1 : -1;
            
            
        targetPos.z += _throwDistance * sign;
        targetPos.y = _floorPoint.position.y;
        
        
        Debug.Log("Бросок!");
        ThrowableObject throwInstance = Instantiate(throwObject);

        
        throwInstance.transform.position = initialPos;
        ObjectThrowed?.Invoke(throwInstance.transform);

  
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested && elapsedTime < _flightDuration) {
            elapsedTime += Time.deltaTime;
            float progress =  elapsedTime / _flightDuration;
            Vector3 newPos = Vector3.Lerp(initialPos, targetPos, progress);
            
            float currentHeight = _height * _throwCurve.Evaluate(progress);
            newPos.y += currentHeight;
            
            throwInstance.transform.position = newPos;
            await UniTask.Yield();
        }
        Debug.Log("Обьект упал! " + throwInstance.transform.position);
        throwInstance.ObjectIsFall();
        await UniTask.WaitForSeconds(1f, cancellationToken: token);
        ObjectFalled?.Invoke();
    }  
    
    private void CalculateInitialDistance() {
        _initialDistance = Vector3.Distance(LeftPoint.position, RightPoint.position) + OffsetToMaxThrow;
        Debug.Log("_initialDistance = " + _initialDistance);
    }
    

    private float CalculateTrajectoryLength(float distance, float height) {
        // Аппроксимация длины дуги параболы
        // Для параболы y = 4*height * (x/distance) * (1 - x/distance)
        // Приближенная формула длины дуги
        float a = distance / 2f;
        float b = height;
    
        // Формула длины дуги параболы (приближенная)
        float term1 = Mathf.Sqrt(a * a + 4 * b * b);
        float term2 = (a * a) / (2 * b) * Mathf.Log((2 * b + term1) / a);

        float trajectoryLength = Mathf.Max((term1 + term2), _minTrajectoryLength);
        Debug.Log("trajectoryLength = " + trajectoryLength);
        return trajectoryLength;
    }
    

    private float CalculateAngleRatio(float angle) {
        float diff = Mathf.Abs(angle - _angleWithMaxDistance);
        diff = Mathf.Clamp(diff, _angleDiapasone.From, _angleWithMaxDistance);
        
        float ratio = 1f - Mathf.Clamp01(diff / _angleWithMaxDistance);
        ratio = Mathf.Max(_minAngleRatio, ratio);
        Debug.Log($"Угол {angle}, ratio: {ratio}");
        return ratio;
    }
}
