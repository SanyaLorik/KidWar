using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Architecture_M;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;

public class ThrowableObject : MonoBehaviour {
    [SerializeField] private Ease _destroyEase;
    [SerializeField] private DOTweenAnimationBase _animation;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private List<Collider> _colliders;
    
    [Header("Анимация удаления")]
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private float _destroySpeed;
    [SerializeField] private float _rotationForceAfterFall = -10f;

    [field: Header("Игровая информация")]
    [field: SerializeField] public InfoThrowableObject Info { get; private set; }

    private IThrowableModifier _modifier;
    private CancellationTokenSource _token;
    private bool _ignoreColliders;
    private float _elapsedTime;
    private CancellationTokenSource _tokenSource;

    public Rigidbody Rb => _rb;
    public Vector3 InitialPos { get; private set; }
    public Vector3 TargetPos { get; private set; }
    public Vector3 EnemyPose { get; private set; }
    public float FlightDuration { get; private set; }
    public float FlightDurationToEnemy { get; private set; }
    public float Height { get; private set; }
    public AnimationCurve ThrowCurve { get; private set; }

    public Transform PointToFollow => transform;
    
    
    /// <summary>
    /// Если был контакт с коллайдером или игроком то не наносить больше урона
    /// </summary>
    private bool _contactPlayer;
    
    public void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out IDamageable player) || _contactPlayer) return;
        if (_modifier != null) {
            _modifier.OnPlayerContact();
            StartDestroyTimer(true);
        }
        player.TakeDamage(Info.Damage);
        _contactPlayer = true;
    }
    
    
    public void OnCollisionEnter(Collision other) {
        StartDestroyTimer(true);
        if (!other.gameObject.TryGetComponent(out ObjectThrower _) && !_ignoreColliders) {
            // Об землю ударилось, сё низя урон наносить
            _contactPlayer = true;
        }
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
        _modifier = modifier;
        _modifier.SetThrowableObject(this);
    }

    public async UniTask StartFlight(CancellationToken token) {
        _elapsedTime = 0f;
        Debug.Log($"TargetPos = {TargetPos}, модификатор: {_modifier.GetType()}");
        _modifier.ExtensionBehaviour();
        _colliders.ForEach(c => c.enabled = false);
        SetStateCollidersAsync(true).Forget();
        while (!token.IsCancellationRequested && _elapsedTime < FlightDuration && !_contactPlayer) {
            _elapsedTime += Time.deltaTime;
            _modifier.CalculatePose(_elapsedTime); 
            await UniTask.WaitForFixedUpdate();
        }
        StartDestroyTimer(true);
    }
    
    
    /// <summary>
    /// Подождать пока коллайдер типа не будет задевать ничего
    /// </summary>
    /// <param name="state"></param>
    private async UniTask SetStateCollidersAsync(bool state) {
        await UniTask.WaitForSeconds(.7f);
        _colliders.ForEach(c => c.enabled = state);
    }
    

    /// <summary>
    /// Запускает 1 раз таймер удаления с анимашкой
    /// </summary>
    /// <param name="state"></param>
    public void StartDestroyTimer(bool state) {
        if (_rb.useGravity == state) return;
        _rb.useGravity = state;
        _tokenSource = new CancellationTokenSource();
        DestroyTimer(_tokenSource.Token).Forget();
    }

    private async UniTask DestroyTimer(CancellationToken token) {
        await UniTask.WaitForSeconds(_timeToDestroy, cancellationToken: token);
        _rb.angularVelocity = new Vector3(
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall)
        );
        transform.DOScale(0f, _destroySpeed)
            .SetEase(_destroyEase)
            .OnComplete(() => Destroy(gameObject));
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
    
    
    
    

    public void Destroy() {
        _elapsedTime = FlightDuration;
        // Полное удаление делает менеджер Calculator он их спавнит и удаляет
        gameObject.DisactiveSelf();
    }

    public void ObjectIsFall() {
        _animation.Kill();
    }

    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}