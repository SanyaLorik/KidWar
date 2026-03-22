using UnityEngine;
using Zenject;

public class PlayerAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int DoubleJump = Animator.StringToHash("doubleJump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    [SerializeField] private Animator _animator;

    [SerializeField] private SkinElementsController _skinElementsController;
    
    [Inject] private PlayerMovement _playerMovement;

    public void SetSkinElementsController(SkinElementsController skinElementsController) {
        _skinElementsController = skinElementsController;
    }
    
    private void OnEnable() {
        _playerMovement.JumpPressed += FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed += SecondJumpAnimation;
        _playerMovement.RunningStateChanged += PlayerMovementOnRunningStateChanged;
        
        
        _playerMovement.Floored += PlayerMovementOnFloored;
    }

    private void PlayerMovementOnFloored() {
        _skinElementsController.EnableShadow();
    }

    private void PlayerMovementOnRunningStateChanged(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void FirstJumpAnimation() {
        _animator.SetTrigger(Jump);
        _skinElementsController.DisableShadow();
    }
    
    
    private void SecondJumpAnimation() {
        _animator.SetTrigger(DoubleJump);
    }

    
    
    private void OnDisable() {
        _playerMovement.JumpPressed -= FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed -= SecondJumpAnimation;
        _playerMovement.RunningStateChanged -= PlayerMovementOnRunningStateChanged;
    }
    
}
