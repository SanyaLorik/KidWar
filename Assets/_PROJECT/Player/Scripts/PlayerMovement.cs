using System;
using Architecture_M;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    [SerializeField] private CharacterController _controller; // 
    
    private Vector2 MoveInput => _inputDirection2.Direction2;
    private float _currentRoll;
    private float _rollVelocity;
    private bool _wasGroundedLastFrame;
    private Vector3 _externalMotion;
    
    
    // Инфа в конфиге была
    private float WalkSpeed;
    private float JumpForce;
    private float SecondJumpForce;
    private float JumpHeight;
    private float RotateSpeed;
    private float GravityScale;
    
    
    public event Action JumpPressed;
    public event Action DoubleJumpPressed;
    public event Action<bool> RunningStateChanged;
    public event Action Floored;
    
    public bool IsGrounded { get; private set; }
    public bool IsRunning { get; private set; }
    public CharacterController Controller => _controller;
    
    
    // [Inject] private PlayerVisual _visual;
    [Inject] private IInputDirection2 _inputDirection2;
    [Inject] private IInputJumping _inputJumping;
    
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
    
    
    // private void Update() {
    //     Walk();
    // }
    //
    // private void OnEnable() {
    //     _inputJumping.OnJumped += OnJump;
    // }
    //
    // private void OnDisable() {
    //     _inputJumping.OnJumped -= OnJump;
    // }

    
    public void TpPlayerInPoint(Transform target) {
        TeleportBase(target.position);
    }
    

    private void TeleportBase(Vector3 point) {
        _controller.enabled = false;
        transform.position = point;
        _controller.enabled = true;
    
        _verticalVelocity = 0; // Сброс скорости
        _jumpsUsed = 0; // Сброс прыжков
    }

    
    public void OnJump() {
        if (_jumpsUsed == 0) {
            _verticalVelocity = JumpForce;
            JumpPressed?.Invoke();
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1) {
            _verticalVelocity = SecondJumpForce;
            DoubleJumpPressed?.Invoke();
            _jumpsUsed = 2;
        }
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

        if (hasInput != IsRunning) {
            IsRunning = hasInput;
            RunningStateChanged?.Invoke(IsRunning);
        }

        // ГРАВИТАЦИЯ
        if (!_controller.isGrounded) {
            _verticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }
        

        Vector3 horizontalMove = hasInput
            ? move.normalized * WalkSpeed * Time.deltaTime
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
                RotateSpeed * Time.deltaTime
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                y,
                transform.eulerAngles.z
            );
        }
    }
    
}