using System.Collections.Generic;
using System.Threading;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThrowableObject : MonoBehaviour {
    [SerializeField] private DOTweenAnimationBase _animation;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _bounceForce = .5f;
    [SerializeField] private float _rotationForceAfterFall = -5f;
    [field: SerializeField] public int Force { get; private set; }
    
    
    private IThrowableModifier _modifier;
    private CancellationTokenSource _token;
    
    public Vector3 InitialPos { get; private set; }
    public Vector3 TargetPos { get; private set; }
    public Vector3 EnemyPose { get; private set; }
    public float FlightDuration { get; private set; }
    public float FlightDurationToEnemy { get; private set; }
    public float Height { get; private set; }
    public AnimationCurve ThrowCurve { get; private set; }
    
    
    public bool ContactPlayer { get; private set; }

    
    /// <summary>
    /// Применяется если модификатор меняет полёт обьекта
    /// </summary>
    /// <param name="duration"></param>
    public void ChangeTimeDuration(float duration) {
        Debug.Log($"Смена FlightDuration с {FlightDuration} на {duration}");
        FlightDuration =  duration;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out ObjectThrower thrower) || ContactPlayer) return;
        if (thrower.IsInvinsible) return;
        thrower.MinusHp(Force);
        ContactPlayer = true;
    }
    

    public void SetupAndLaunch( 
        Vector3 initialPos, 
        Vector3 targetPos, 
        Vector3 enemyPose, 
        float flightDuration, 
        float flightDurationToEnemy, 
        float height, 
        AnimationCurve throwCurve,
        IThrowableModifier modifier
    ) {
        InitialPos = initialPos;
        TargetPos = targetPos;
        EnemyPose = enemyPose;
        FlightDuration = flightDuration;
        FlightDurationToEnemy = flightDurationToEnemy;
        Height = height;
        ThrowCurve = throwCurve;
        _modifier = modifier == null ? new ThrowableModifierDefault() : modifier;
        _modifier.SetThrowableObject(this);
    }
    
    public async UniTask StartFlight(CancellationToken token) {
        float elapsedTime = 0f;
        Debug.Log($"TargetPos = {TargetPos}, модификатор: {_modifier.GetType()}");
        _modifier.ExtensionBehaviour();
        while (!token.IsCancellationRequested && elapsedTime < FlightDuration && !ContactPlayer) {
            elapsedTime += Time.deltaTime;
            _modifier.CalculatePose(elapsedTime); 
            await UniTask.WaitForFixedUpdate();
        }
    }


    private void OnCollisionEnter(Collision other) {
        EnablePhysicsAfterHit(other);
        if (!other.gameObject.TryGetComponent(out ObjectThrower thrower)) {
            // Об землю ударилось, сё низя урон наносить
            ContactPlayer = true;
        }
    }

    private void EnablePhysicsAfterHit(Collision collision) {
        // Включаем гравитацию
        _rb.useGravity = true;
        
        // Отталкиваемся от точки столкновения
        ContactPoint contact = collision.contacts[0];
        Vector3 bounceDir = Vector3.Reflect(_rb.linearVelocity, contact.normal);
        bounceDir.y = Mathf.Abs(bounceDir.y) + 2f; // Добавляем отскок вверх

        _rb.linearVelocity = bounceDir * _bounceForce; // Отскок с силой 3
        
        // Добавляем вращение
        _rb.angularVelocity = new Vector3(
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall)
        );
        // Уничтожаем через 3 секунды
    }

    public void ObjectIsFall() {
        _animation.Kill();
    }

    public void ChangeObjectForce(int force) {
        Force =  force;
    }

    public void ChangeObjectSize() {}


    
}