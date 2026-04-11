using System;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public enum PlayerState {
    Play,
    InSpawn
}


public class PlayerStateManager : MonoBehaviour{
    [SerializeField] private JumpParticlesController _flooringParticlesController;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    
    [Header("Player")]
    [SerializeField] private GameObject[] _objectsToShowInPlay;
    [SerializeField] private GameObject[] _objectsToHideInPlay;
    [SerializeField] private GameObject[] _canvases;

    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private IInterstitialActivity  _interstitialActivity;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private BattleManager _battleManager;
    [Inject] private IGameSave _saver;

    [InjectOptional] private IActivityButtonPC _activityButtonPC;
    [Inject] private ThrowGameStarter _throwGameManager;
    
    public event Action<PlayerState> StateChanged;
    
    private void OnEnable() {
        _playerMovement.Floored += PlayerMovementOnFloored;
        _playerMovement.JumpPressed += PlayerMovementOnJumpPressed;
        _playerMovement.DoubleJumpPressed += PlayerMovementOnJumpPressed;
    }

    public void SetupCanvases(bool playerGoPlay) {
        MobileInputHide(playerGoPlay);
        _canvases.DisactiveSelf();
        if (playerGoPlay) {
            // _playContainer.ActiveSelf();
            if(_objectsToShowInPlay.Length != 0) _objectsToShowInPlay.ActiveSelf();
            if(_objectsToHideInPlay.Length != 0) _objectsToHideInPlay.DisactiveSelf();
        }
        else {
            if(_objectsToShowInPlay.Length != 0) _objectsToShowInPlay.DisactiveSelf();
            if(_objectsToHideInPlay.Length != 0) _objectsToHideInPlay.ActiveSelf();
            // _playContainer.DisactiveSelf();
        }
        ChangePlayerState(playerGoPlay ? PlayerState.Play : PlayerState.InSpawn);
    }

    private void MobileInputHide(bool hide) {
        if (_activityButtonPC == null) return;
        if (hide) {
            _activityButtonPC.HideJumpButton();
            _activityButtonPC.HidOrbitalJoystick();
        }
        else {
            _activityButtonPC.ShowJumpButton();
            _activityButtonPC.ShowOrbitalJoystick();
        }
    }
    
    

    private void PlayerMovementOnJumpPressed() {
        _jumpParticlesController.Play();
    }


    private void PlayerMovementOnFloored() {
        _flooringParticlesController.Play();
    }


    private void Start() {
        if (_saver.GetSave<GameSave>().IsBoughtPurchase) {
            _interstitialActivity.DisableInterstitial();
        }
        SetupCanvases(false);
    }


    public PlayerState CurrentState { get; private set; } = PlayerState.InSpawn;
    public PlayerState BeforeState { get; private set; } = PlayerState.InSpawn;

    private void ChangePlayerState(PlayerState newState) {
        if(newState == CurrentState) return;
        BeforeState = CurrentState;
        CurrentState = newState;
        StateChanged?.Invoke(newState);
    }

}


