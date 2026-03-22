using System;
using Architecture_M;
using UnityEngine;
using Zenject;

public enum PlayerState {
    Play,
    InSpawn
}


public class PlayerStateManager : MonoBehaviour{
    [SerializeField] private JumpParticlesController _flooringParticlesController;
    [SerializeField] private JumpParticlesController _jumpParticlesController;

    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private IInterstitialActivity  _interstitialActivity;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] protected IGameSave _saver;

    [InjectOptional] private IActivityButtonPC _activityButtonPC;
    
    public event Action<PlayerState> ChangeState;
    
    private void OnEnable() {
        _playerMovement.Floored += PlayerMovementOnFloored;
        _playerMovement.JumpPressed += PlayerMovementOnJumpPressed;
        _playerMovement.DoubleJumpPressed += PlayerMovementOnJumpPressed;
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
    }


    public PlayerState CurrentState { get; private set; } = PlayerState.InSpawn;
    public PlayerState BeforeState { get; private set; } = PlayerState.InSpawn;

    
    
    public void ChangePlayerState(PlayerState newState) {
       
        ChangeState?.Invoke(CurrentState);
    }

    private void HideFlightMobileView(bool state) {
        if(_activityButtonPC == null) return;
        if (state) {
            _activityButtonPC.HideJumpButton();
            _activityButtonPC.HidOrbitalJoystick();
        }
        else {
            _activityButtonPC.ShowJumpButton();
            _activityButtonPC.ShowOrbitalJoystick();
        }
    }

}
