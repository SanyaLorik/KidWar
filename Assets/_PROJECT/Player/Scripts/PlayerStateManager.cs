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
    [SerializeField] private GameObject _playContainer;
    [SerializeField] private GameObject[] _objectsToShowInPlay;
    [SerializeField] private GameObject[] _objectsToHideInPlay;

    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private IInterstitialActivity  _interstitialActivity;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private BattleManager _battleManager;
    [Inject] protected IGameSave _saver;

    [InjectOptional] private IActivityButtonPC _activityButtonPC;
    [Inject] private ThrowGameStarter _throwGameManager;
    
    public event Action<PlayerState> ChangeState;
    
    private void OnEnable() {
        _playerMovement.Floored += PlayerMovementOnFloored;
        _playerMovement.JumpPressed += PlayerMovementOnJumpPressed;
        _playerMovement.DoubleJumpPressed += PlayerMovementOnJumpPressed;
    }

    public void SetupCanvases(bool playerGoPlay) {
        ChangePlayerState(playerGoPlay ? PlayerState.Play : PlayerState.InSpawn);
        if (playerGoPlay) {
            _playContainer.ActiveSelf();
            _objectsToShowInPlay.ActiveSelf();
            _objectsToHideInPlay.DisactiveSelf();
        }
        else {
            _objectsToShowInPlay.DisactiveSelf();
            _objectsToHideInPlay.ActiveSelf();
            _playContainer.DisactiveSelf();
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
        ChangeState?.Invoke(newState);
    }

    // будет настройка канваса для игрока
    private void HideFlightMobileView(bool state) {
        
    }

}
