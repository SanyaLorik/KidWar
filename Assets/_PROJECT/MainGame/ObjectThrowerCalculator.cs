using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

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
    
    
    [Header("Траектория игрока 1")]
    [SerializeField] private TrajectoryVisualize3D _playerAngle;
    
    [Header("Диапазон угла бросания")]
    [SerializeField] private PairedValue<float> _angleDiapasone;
    [Header("Угол при котором будет максимальный бросок")]
    [SerializeField] private float _angleWithMaxDistance;
    [Header("Коэффициенты меньшего и большего угла")]
    [SerializeField] private float _minAngleRatio;
    
    
    [SerializeField] private GameObject _throwObject;
    [SerializeField] private Transform _playerThrowPoint;
    [SerializeField] private Transform _floorPoint;
    
    [SerializeField] private float _testHeight;
    
    [Header("Скорость полёта")] 
    [SerializeField] private float _fySpeed;
    
    
    
    private Mouse _mouse;
    private bool _allowToThrow = true;
    private float _initialDistance;
    private CancellationTokenSource _tokenSource;
    
    
    public event Action<Transform> ObjectThrowed;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] PlayersIniter _playersIniter;


    void Start() {
        _mouse = Mouse.current;
        CalculateInitialDistance();
    }
    
    private void Update() {
        if (_mouse.leftButton.wasReleasedThisFrame && _allowToThrow) {
            ThrowNewObject();
        }
    }

    // Вынести
    private void CalculateInitialDistance() {
        _initialDistance = Vector3.Distance(_playersIniter.LeftPoint.position, _playersIniter.RightPoint.position) 
                           + _playersIniter.OffsetToMaxThrow;
        Debug.Log("_initialDistance = " + _initialDistance);
    }
    

    private void ThrowNewObject() {
        _allowToThrow = false;
        float distance = CalculateThrowDistance();
        float height = CalculateHeight();

        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ThrowObject(distance, height, _tokenSource.Token).Forget();
    }

    private float CalculateThrowDistance() {
        CalculateInitialDistance();
        float angleRatio = CalculateAngleRatio(_playerAngle.CurrentVerticalAngle);
        float distance = _initialDistance * _throwForce * angleRatio + _windForce;
        Debug.Log("distance " + distance);
        return distance;
    }

    private float CalculateHeight() {
        float height;
        height = _testHeight * _playerAngle.CurrentVerticalAngle / _angleDiapasone.To;
        return height;
    }
    
    private async UniTask ThrowObject(float distance, float height, CancellationToken token) {
        // задержка перед броском, можна для анимации
        await UniTask.WaitForSeconds(.3f, cancellationToken: token);
        // Кидаем пока из точки игрока в правую точку
        Vector3 initialPos = _playerThrowPoint.position;
        Vector3 targetPos = initialPos;
        targetPos.z += distance;
        targetPos.y = _floorPoint.position.y;
        
        Debug.Log("Бросок!");
        Transform throwInstance = Instantiate(_throwObject.transform);
        throwInstance.position = initialPos;
        ObjectThrowed?.Invoke(throwInstance);

        float trajectoryLength = CalculateTrajectoryLength(distance, height);
        float flightDuration = trajectoryLength / _fySpeed;
        
        
        
        float elapsedTime = 0f;
        while (!token.IsCancellationRequested && elapsedTime < flightDuration) {
            elapsedTime += Time.deltaTime;
            float progress =  elapsedTime / flightDuration;
            Vector3 newPos = Vector3.Lerp(initialPos, targetPos, progress);
            
            float currentHeight = height * _throwCurve.Evaluate(progress);
            newPos.y += currentHeight;
            
            throwInstance.transform.position = newPos;
            await UniTask.Yield();
        }
        Debug.Log("Обьект упал! " + throwInstance.transform.position);
        _allowToThrow = true;
        await UniTask.WaitForSeconds(1f, cancellationToken: token);
        ObjectThrowed?.Invoke(_playerMovement.transform);
    }  
    
    
    // Метод для аппроксимации длины траектории
    private float CalculateTrajectoryLength(float distance, float height) {
        // Аппроксимация длины дуги параболы
        // Для параболы y = 4*height * (x/distance) * (1 - x/distance)
        // Приближенная формула длины дуги
        float a = distance / 2f;
        float b = height;
    
        // Формула длины дуги параболы (приближенная)
        float term1 = Mathf.Sqrt(a * a + 4 * b * b);
        float term2 = (a * a) / (2 * b) * Mathf.Log((2 * b + term1) / a);
    
        return term1 + term2;
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
