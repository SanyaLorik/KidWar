using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BotAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    [SerializeField] private Animator _animator;

    private BotWander _botWander;
    private SkinElementsController _skinController;
    private CancellationTokenSource _tokenSource;
    
    public void SetModelData(Avatar avatar, SkinElementsController controller) {
        _animator.avatar = avatar;
        _skinController = controller;
    }


    public void InitAnimator(BotWander botWander) {
        _botWander = botWander;
        _botWander.OnJump += OnJump;
        _botWander.StartWandering += OnStartWandering;
        _botWander.Grounded += BotGrounded;
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
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        SetWalkLogicLater(isRunning, _tokenSource.Token).Forget();
    }

    private async UniTask SetWalkLogicLater(bool isRunning, CancellationToken token) {
        // Ждем 5 кадров через счетчик чтоб анимка заработала
        int framesToWait = 5;
        await UniTask.WaitUntil(() => 
        {
            framesToWait--;
            return framesToWait <= 0;
        }, cancellationToken: token);
        
        
        _animator.SetBool(Run, isRunning);
        if (_skinController!=null) {
            _skinController.EnableShadow();
        }
    }

    private void OnJump() {
        _animator.SetTrigger(Jump);
    }
}