using System;
using System.Collections;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DualLegParticles : MonoBehaviour {
    [SerializeField] private ParticleSystem _ps;
    private ParticleSystem.EmissionModule _emission;
    
    [SerializeField] private bool _botBehaviour;
    
    private CancellationTokenSource _tokenSource;
    private bool _allowToPlay = true;

    [Inject] private PlayerMovement _playerMovement;
    
    private void Awake() {
        _emission = _ps.emission;
        StartCoroutine(StartSystem());
    }
    
    private void OnEnable() {
        _playerMovement.JumpPressed += PlayerMovementOnOnJumpPressed;
        _playerMovement.DoubleJumpPressed += PlayerMovementOnOnJumpPressed;
        _playerMovement.Floored += PlayerMovementOnFloored;
    }

    public bool IsPlaying { get; private set; }

    public void Play() {
        IsPlaying = true;
        StartRunning();
    }

    
    public void Stop() {
        IsPlaying = false;
        StopRunning();
    }
    
    private void PlayerMovementOnFloored() {
        _allowToPlay = true;
    }

    private void PlayerMovementOnOnJumpPressed() {
        _allowToPlay = false;
    }
    
    private IEnumerator StartSystem() {
        StartRunning();
        yield return null;
        StopRunning();
        _tokenSource = new CancellationTokenSource();
        if (!_botBehaviour) {
            PlayerLogic(_tokenSource.Token).Forget();
        }
    }

    private async UniTask PlayerLogic(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            bool allowPlay = _playerMovement.AllowMove;

            if (allowPlay && IsPlaying || !_allowToPlay) {
                IsPlaying = false;
                StopRunning();
            }
            else if (allowPlay && !IsPlaying && _allowToPlay) {
                IsPlaying = true;
                StartRunning();
            }
            await UniTask.Yield(token);
        }
    }
    
    

    private void StartRunning() {
        _emission.enabled = true; 
    }

    private void StopRunning() {
        _emission.enabled = false; 
    }

    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}
