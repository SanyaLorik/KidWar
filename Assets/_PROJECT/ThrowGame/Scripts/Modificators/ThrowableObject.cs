using System.Threading;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
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
    private bool _ignoreColliders;
    private float _elapsedTime;

    public Rigidbody Rb => _rb;
    public Vector3 InitialPos { get; private set; }
    public Vector3 TargetPos { get; private set; }
    public Vector3 EnemyPose { get; private set; }
    public float FlightDuration { get; private set; }
    public float FlightDurationToEnemy { get; private set; }
    public float Height { get; private set; }
    public AnimationCurve ThrowCurve { get; private set; }
    public bool ObjectIsFalled { get; private set; }

    /// <summary>
    /// Если был контакт с коллайдером или игроком то не наносить больше урона
    /// </summary>
    public bool ContactPlayer { get; private set; }
    
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
        _modifier = modifier;
        _modifier.SetThrowableObject(this);
    }

    public async UniTask StartFlight(CancellationToken token) {
        _elapsedTime = 0f;
        Debug.Log($"TargetPos = {TargetPos}, модификатор: {_modifier.GetType()}");
        _modifier.ExtensionBehaviour();
        while (!token.IsCancellationRequested && _elapsedTime < FlightDuration && !ContactPlayer) {
            _elapsedTime += Time.deltaTime;
            _modifier.CalculatePose(_elapsedTime); 
            await UniTask.WaitForFixedUpdate();
        }
        SetGravity(true);
    }

    public void SetFallStatus(bool state) {
        ObjectIsFalled = state;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out IDamageable player) || ContactPlayer) return;
        if (_modifier != null) {
            _modifier.OnPlayerContact();
        }
        player.TakeDamage(Force);
        ContactPlayer = true;
    }
    
    
    private void OnCollisionEnter(Collision other) {
        EnablePhysicsAfterHit(other);
        if (!other.gameObject.TryGetComponent(out ObjectThrower _) && !_ignoreColliders) {
            // Об землю ударилось, сё низя урон наносить
            ContactPlayer = true;
        }
    }

    public void SetGravity(bool state) {
        if (_rb.useGravity != state) {
            _rb.useGravity = state;
        }
    }

    

    /// <summary>
    /// Если обьект ударился об пол он всеровно нанесёт урон игроку пока использует только модификатор взрыва
    /// </summary>
    public void SetIgnoreOtherColliders() {
        _ignoreColliders = true;
    }

        
    /// <summary>
    /// Применяется если модификатор меняет полёт обьекта
    /// </summary>
    /// <param name="duration"></param>
    public void ChangeTimeDuration(float duration) {
        Debug.Log($"Смена FlightDuration с {FlightDuration} на {duration}");
        FlightDuration =  duration;
    }
    
    private void EnablePhysicsAfterHit(Collision collision) {
        // Включаем гравитацию
        SetGravity(true);
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
    }

    public void Destroy() {
        _elapsedTime = FlightDuration;
        // Полное удаление делает менеджер Calculator он их спавнит и удаляет
        gameObject.DisactiveSelf();
    }

    public void ObjectIsFall() {
        _animation.Kill();
    }
    
}