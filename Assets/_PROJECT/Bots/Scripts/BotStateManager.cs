using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum BotState {
    Wandering
}

[RequireComponent(typeof(NavMeshAgent))]
public class BotStateManager : MonoBehaviour, IThrowGamePlayer {
    [SerializeField] private Transform _skinParent;
    [SerializeField] private BotAnimator _botAnimator;
    [SerializeField] private GameObject _skinInstance;
    [SerializeField] private Collider _botCollider;

    [SerializeField] private ObjectThrower _thrower;
    private Vector3 _posBeforeTeleport;
    
    
    private BotWander _botWander;
    private BotMonolog _botMonolog;
    
    private IBotBehaviour _currentBotBehaviour;
    private NavMeshAgent _agent;

    public bool IsPlaying { get; private set; }
    
    public BotState State { get; private set; }
    
    public ObjectThrower ObjectThrower => _thrower;

    private void Awake() {
        _botWander = GetComponent<BotWander>();
        _botMonolog = GetComponent<BotMonolog>();
        _agent = GetComponent<NavMeshAgent>();
        _botCollider.enabled = false;
        Destroy(_skinInstance);
    }
    
    
    
    public void SetPlayStatus(bool goPlay) {
        Debug.Log("SetPlayStatus: " + goPlay);
        Debug.Log("_posBeforeTeleport: " + _posBeforeTeleport);
        IsPlaying = goPlay;
        if (goPlay) {
            _currentBotBehaviour?.Exit();
            _botCollider.enabled = true;
        }
        // Возвращение на спавн
        else {
            TpInPoint(_posBeforeTeleport);
            _botCollider.enabled = false;
            ChangeBotState(BotState.Wandering);
        }
    }

    public void SetPlayStatusSilent(bool goPlay) {
        IsPlaying =  goPlay;
    }


    public void TpInPoint(Vector3 pos) {
        _posBeforeTeleport = transform.position;
        Debug.Log("TpInPoint bot");
        // _agent.enabled = false;
        if (NavMesh.SamplePosition(pos, out var hit, 5f, NavMesh.AllAreas)) {
            _agent.Warp(hit.position);
        }
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