using System;
using System.Collections;
using UnityEngine;

public enum BotState {
    Wandering
}


public class BotStateManager : MonoBehaviour {
    [SerializeField] private Transform _skinParent;
    [SerializeField] private BotAnimator _botAnimator;
    [SerializeField] private GameObject _skinInstance;
    
    private BotWander _botWander;
    private BotMonolog _botMonolog;
    
    private IBotBehaviour _currentBotBehaviour;

    public BotState State { get; private set; }
    public Rigidbody Rb { get; private set; }

    private void Awake() {
        _botWander = GetComponent<BotWander>();
        _botMonolog = GetComponent<BotMonolog>();
        Rb = GetComponent<Rigidbody>();
        Destroy(_skinInstance);
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
