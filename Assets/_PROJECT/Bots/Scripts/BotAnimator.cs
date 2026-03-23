using UnityEngine;

public class BotAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    [SerializeField] private Animator _animator;

    private BotWander _botWander;
    private SkinElementsController _skinController;
    
    public void SetModelData(Avatar avatar, SkinElementsController controller) {
        _animator.avatar = avatar;
        _skinController = controller;
        Debug.Log("SetModelData");
    }


    public void InitAnimator(BotWander botWander) {
        _botWander = botWander;
        _botWander.OnJump += OnJump;
        _botWander.StartWandering += OnStartWandering;
        _botWander.Grounded += BotGrounded;
        Debug.Log("InitAnimator");
    }

    private void BotGrounded(bool grounded) {
        if(_skinController == null) return;
        if (grounded) {
            _skinController.EnableShadow();
        }
        else {
            _skinController.DisableShadow();
        }
    }

    
    private void OnStartWandering(bool isRunning) {
        _animator.SetBool(Run, isRunning);
        _animator.Update(0f);
        bool actualValue = _animator.GetBool(Run);
        Debug.Log($"OnStartWandering: установил {isRunning}, реально в аниматоре: {actualValue}");
        Debug.Log("OnStartWandering:  " + isRunning);
        if (_skinController!=null) {
            _skinController.EnableShadow();
        }
    }

    private void OnJump() {
        _animator.SetTrigger(Jump);
    }
}