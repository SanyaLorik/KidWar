using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum BotState {
    Wandering
}

[RequireComponent(typeof(NavMeshAgent))]
public class BotStateManager : MonoBehaviour {
    [SerializeField] private Transform _skinParent;
    [SerializeField] private BotAnimator _botAnimator;
    [SerializeField] private GameObject _skinInstance;
    [field: SerializeField] public ObjectThrower BotThrower { get; private set; }
    private Vector3 _lastSpawnPoint;
    
    
    private BotWander _botWander;
    private BotMonolog _botMonolog;
    
    private IBotBehaviour _currentBotBehaviour;
    private NavMeshAgent _agent;
    private Collider _botCollider;

    public bool IsPlaying { get; private set; }
    
    public BotState State { get; private set; }

    private void Awake() {
        _botWander = GetComponent<BotWander>();
        _botMonolog = GetComponent<BotMonolog>();
        _agent = GetComponent<NavMeshAgent>();
        _botCollider = GetComponent<Collider>();
        Destroy(_skinInstance);
    }
    
    
    
    public void SetBotPlayStatus(bool goPlay) {
        if (goPlay) {
            _currentBotBehaviour?.Exit();
            _lastSpawnPoint = transform.position;
            _botCollider.enabled = true;
        }
        // Возвращение на спавн
        else if (IsPlaying) {
            TpInPoint(_lastSpawnPoint);
            _botCollider.enabled = false;
        }
        IsPlaying = goPlay;
        _agent.enabled = !goPlay;
    }

    public void TpInPoint(Vector3 pos) {
        _agent.enabled = false;
        transform.position = pos;
    }
    
    public void RotateToTarget(Vector3 targetPosition) {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    public void ReturnToWandering() {
        
    }

    private void Start() {
        ChangeBotState(BotState.Wandering);
    }


    public void ChangeBotState(BotState newState) {
        _currentBotBehaviour?.Exit();
        
        State = newState;
        _currentBotBehaviour = State switch {
            BotState.Wandering => _botWander,
            _ => _currentBotBehaviour
        };

        // Debug.Log(_currentBotBehaviour);
        _currentBotBehaviour?.Enter();
    }
    
    public void ChangeNickname() {
        // _botMonolog.ChangeNickname();
    }



    public void InitAnimator() {
        _botAnimator.InitAnimator(_botWander);
    }
    public void SetBotSkin(SkinItemConfig skinItemConfig) {
        StartCoroutine(ChangeSkinRoutine(skinItemConfig));
    }
    
    private IEnumerator ChangeSkinRoutine(SkinItemConfig skin) {
        if (_skinInstance != null) {
            Destroy(_skinInstance);
            _botAnimator.SetModelData(null, null);
        }
        yield return null; // дождаться конца кадра

        _skinInstance = Instantiate(skin.SkinPrefab, _skinParent);
        var skinItem = _skinInstance.GetComponent<SkinElementsController>();
        _botAnimator.SetModelData(skin.Avatar, skinItem);
    }
    

    public void SetBotSpeak() {
        _botMonolog.SaySomething();
    }

    public void SetBotStfu() {
        _botMonolog.Stfu();
    }

}
