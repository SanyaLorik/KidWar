using System;
using System.Collections;
using System.Diagnostics;
using Architecture_M;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IThrowGamePlayer {
    [SerializeField] private CharacterController _controller; // 
    [SerializeField] private ObjectThrower _thrower;
    [SerializeField] private Transform _spawnPoint;
    
    // [SerializeField] private PlayerVisual _visual;

    public Vector2 MoveInput => _inputDirection2.Direction2;
    public bool AllowMove => _inputDirection2.Direction2 != Vector2.zero && !IsPlaying;
    private float _currentRoll;
    private float _rollVelocity;
    private bool _wasGroundedLastFrame;
    private Vector3 _externalMotion;
    
    
    public event Action JumpPressed;
    public event Action DoubleJumpPressed;
    public event Action<bool> RunningStateChanged;
    public event Action Floored;
    
    public bool IsGrounded { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPlaying { get; private set; }
    
    public CharacterController Controller => _controller;
    
    
    [Inject] private IInputDirection2 _inputDirection2;
    [Inject] private IInputActivity _inputActivity;
    [Inject] private IInputJumping _inputJumping;
    [Inject] private GameData _gameData;
    [Inject] private PlayerStateManager _stateManager;
    
    // Для гравитации и прыжков
    private float _verticalVelocity;
    private int _jumpsUsed;

    /// <summary>
    /// Какой-то внешний стимулятор движения, например лифт
    /// </summary>
    /// <param name="motion"></param>
    public void AddExternalMotion(Vector3 motion) {
        _externalMotion += motion;
    }

    private void SetCharacterControllerState(bool state) {
        _controller.enabled = state;
    }
    
    private void Update() {
        Walk();
    }
    
    private void OnEnable() {
        _inputJumping.OnJumped += OnJump;
    }
    
    
    private void OnDisable() {
        _inputJumping.OnJumped -= OnJump;
    }


    public void TpInPoint(Vector3 target) {
        SetCharacterControllerState(false);
        transform.position = target;
        SetCharacterControllerState(true);
    
        _verticalVelocity = 0; // Сброс скорости
        _jumpsUsed = 0; // Сброс прыжков
    }

    public void SetPlayStatus(bool goPlay) {
        _stateManager.SetupCanvases(goPlay);
        // игрок не учавствовал в бою
        // Вернулся
        if (!goPlay) {
            TeleportInSpawn();
            _inputActivity.Enable();
            SetCharacterControllerState(true);
            IsPlaying = false;
        }
        else {
            _inputActivity.Disable();
            IsPlaying = true;
            StartCoroutine(OffAnimationsRoutine());
        }
    }

    public void TeleportInSpawn() {
        TpInPoint(_spawnPoint.position);
    }


    public ObjectThrower ObjectThrower => _thrower;

    private Coroutine _controllerOffRoutine;
    private IEnumerator OffAnimationsRoutine() {
        yield return new WaitForSeconds(.5f);
        IsRunning = false;
        RunningStateChanged?.Invoke(IsRunning);
    }


    private float _lastJumpTime;
    private const float JumpCooldown = 0.1f; // 100 мс
    public void OnJump() {
        if (Time.time - _lastJumpTime < JumpCooldown)
            return;

        _lastJumpTime = Time.time;

        if (_jumpsUsed == 0) {
            _verticalVelocity = _gameData.JumpForce;
            JumpPressed?.Invoke();
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1) {
            _verticalVelocity = _gameData.SecondJumpForce;
            DoubleJumpPressed?.Invoke();
            _jumpsUsed = 2;
        }
    }


    public void RotateToTarget(Vector3 targetPosition) {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        SetCharacterControllerState(false);
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
        SetCharacterControllerState(true);
    }


    private void Walk() {
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * MoveInput.x + camForward * MoveInput.y;
        bool hasInput = move.sqrMagnitude > 0.001f;

        if (IsPlaying) {
            move = Vector3.zero;
        }
        
        if (hasInput != IsRunning) {
            IsRunning = hasInput;
            RunningStateChanged?.Invoke(IsRunning && !IsPlaying);
        }

        // ГРАВИТАЦИЯ
        if (!_controller.isGrounded) {
            _verticalVelocity += Physics.gravity.y * _gameData.GravityScale * Time.deltaTime;
        }
        

        Vector3 horizontalMove = hasInput
            ? move.normalized * _gameData.WalkSpeed * Time.deltaTime
            : Vector3.zero;

        Vector3 verticalMove = Vector3.up * _verticalVelocity * Time.deltaTime;

        _controller.Move(horizontalMove + verticalMove + _externalMotion);
        _externalMotion = Vector3.zero;
        // Проверяем grounded ПОСЛЕ Move
        IsGrounded = _controller.isGrounded;

        
        // Прилипание к земле (анти-дребезг)
        if (IsGrounded) {
            if (_verticalVelocity < 0f) {
                _verticalVelocity = -2f;
            }
        }

        // Настоящее приземление
        bool justLanded = IsGrounded && !_wasGroundedLastFrame && _verticalVelocity < -0.1f;

        if (justLanded) {
            _jumpsUsed = 0;
            Floored?.Invoke();
        }

        _wasGroundedLastFrame = IsGrounded;

        if (hasInput) {
            WalkRotate(move);
        }
    }
    
    
    private void WalkRotate(Vector3 move) {
        if (move.sqrMagnitude > 0.0001f) {
            float TargetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            
            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                TargetPosY,
                _gameData.RotateSpeed * Time.deltaTime
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                y,
                transform.eulerAngles.z
            );
        }
    }
    

    
}