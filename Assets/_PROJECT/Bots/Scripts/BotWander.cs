using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class BotWander : MonoBehaviour, IBotBehaviour {
    [Header("Партиклы")]
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private JumpParticlesController _landParticleController;
    [SerializeField] private DualLegParticles _walkingParticles;

    
    public bool Eblaning { get; private set; }
    
    public Action<bool> StartWandering;
    public Action OnJump;
    public Action<bool> Grounded;
    private Transform _chooseCube;
    private CancellationTokenSource _botTokenSource;
    private NavMeshAgent _agent;
    
    [Inject(Id = "WalkPoints")] private Transform[] _pointsToWalk;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private GameData _gameData;
    

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }


    public void Enter() {
        _botTokenSource = new CancellationTokenSource();
        Eblaning = true;
        // Сначала уведомить о начале движения
        StartWandering?.Invoke(true); // ← Вызвать ДО старта асинхронных задач
        _walkingParticles.Play();

        // Потом запускать циклы
        LifeCycleAsync(_botTokenSource.Token).Forget();
        MonitorMovementAsync(_botTokenSource.Token).Forget();
    }
    
    public void Exit() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
        _botTokenSource =  null;
        _walkingParticles.Stop();
        Eblaning = false;
        StartWandering?.Invoke(false);
    }

    private async UniTask MonitorMovementAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            if (_agent.enabled && _agent.velocity.sqrMagnitude > 0.05f) {
                if (!_walkingParticles.IsPlaying) {
                    _walkingParticles.Play();
                    StartWandering?.Invoke(true);
                }
            }
            else {
                if (_walkingParticles.IsPlaying) {
                    _walkingParticles.Stop();
                    StartWandering?.Invoke(false);
                }
            }

            await UniTask.Yield(token);
        }
    }
    
    
    


    private async UniTask LifeCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            
            Vector3 target = ChooseNextTarget();
            _agent.SetDestination(target);
            _agent.stoppingDistance = Random.Range(
                _gameData.StoppingDistance.From,
                _gameData.StoppingDistance.To);
            
            
            await UniTask.WaitUntil(() => !_agent.pathPending && _agent.hasPath, cancellationToken: token);
            Jump(token).Forget();

            await UniTask.WaitUntil(() => 
                !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance,
                cancellationToken: token);
            
            await RotateTowardsAsync(target, _gameData.RotationSpeed, token);

            float waitTime = Random.Range(
                _gameData.TimeToStayOnPoint.From, 
                _gameData.TimeToStayOnPoint.To);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
    }

    private async UniTask Jump(CancellationToken token) {
        if (Random.value > _gameData.ChanceToJump) return;
        
        float startPathLength = _agent.remainingDistance;
        float jumpLength = startPathLength / Random.Range(1.5f, 2f);

        await UniTask.WaitUntil(() => 
                !_agent.pathPending &&
                _agent.remainingDistance <= jumpLength &&
                _agent.remainingDistance > _agent.stoppingDistance, 
            cancellationToken: token);

        FakeJump(token).Forget();
    }
    
    [SerializeField] private float _jumpDuration;
    private async UniTask FakeJump(CancellationToken token) {
        float height = _gameData.JumpForce / 2f;
        float t = 0f;

        _jumpParticlesController.Play();
        OnJump?.Invoke();
        Grounded?.Invoke(false);
        while (t < _jumpDuration) {
            t += Time.deltaTime;
            float normalized = t / _jumpDuration;
            float yOffset = Mathf.Sin(normalized * Mathf.PI) * height;

            Vector3 pos = transform.position;
            pos.y = _agent.nextPosition.y + yOffset;

            transform.position = pos;

            await UniTask.Yield(token);
        }
        Grounded?.Invoke(true);
        _landParticleController.Play();
    }

    

    private Vector3 ChooseNextTarget() {
        float rv = Random.value;
        if (_playerStateManager.CurrentState == PlayerState.Play &&  rv < _gameData.ChanseToGoPlayer)
            return _playerMovement.transform.position;

        // Иначе выбираем случайный куб
        return GetTargetPoint(_pointsToWalk.GetRandomElement());
    }
    
    private Vector3 GetTargetPoint(Transform point) {
        Vector3 size = point.localScale;

        float offsetX = Random.Range(-size.x/2f - 2f, size.x/2f + 2f);
        float offsetZ = Random.Range(-size.z/2f - 2f, size.z/2f + 2f);

        Vector3 target = point.position + new Vector3(offsetX, 0f, offsetZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas)) {
            // Debug.Log("Цель бота: " + hit.position);
            return hit.position;
        }

        // Если не нашли на навмеш, просто центр куба
        // Debug.Log("Цель бота: " + point.position);
        return point.position;
    }

    
    private async UniTask RotateTowardsAsync(Vector3 target, float rotationSpeed, CancellationToken token) {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; // Игнорируем разницу по высоте
    
        if (direction == Vector3.zero) return;
    
        Quaternion targetRotation = Quaternion.LookRotation(direction);
    
        // Плавный поворот
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f && !token.IsCancellationRequested) {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
            await UniTask.Yield(token);
        }
    }
    
    private void OnDestroy() {
        _botTokenSource?.Cancel();
        _botTokenSource?.Dispose();
    }
    
}
