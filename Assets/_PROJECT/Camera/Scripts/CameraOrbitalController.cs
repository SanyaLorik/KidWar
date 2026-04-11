using System;
using System.Collections;
using Architecture_M;
using SanyaBeerExtension;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraOrbitalController : MonoBehaviour {
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    
    [Header("Точка движения игрока")]
    [SerializeField] private Transform _walkPoint; 


    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
    [SerializeField] private float _cameraSaveDelay = 1f;
    [SerializeField] private Vector3 _dampningWhlePlay;
    [SerializeField] private Vector3 _dampningInWinnerWindow;
    [SerializeField] private CinemachineBasicMultiChannelPerlin _noise;
    [SerializeField] private float _intensity;
    [SerializeField] private float _frequency;
    [SerializeField] private float _duration;
    
    
    private Action _rotationHandler;

    private bool IsMobile => _inputType == InputType.Mobile;


    private Mouse _mouse;
    private bool _isOrbiting;


    private bool _allowRotation = true;
    private bool _allowZoom = true;
    private float _defaultX;
    private float _defaultY;

    public float DefaultFov => IsMobile ? _gameData.MobileCameraFov : _gameData.DesktopCameraFov;
    public float CurrentFovPercent => (_orbitalFollow.RadialAxis.Value - _gameData.ZoomDiapasone.From) 
                                      / 
                                      (_gameData.ZoomDiapasone.To - _gameData.ZoomDiapasone.From);
    public float DefaultSens => _gameData.DefaultCameraSens;

    private float Sensitivity => Mathf.Clamp(_settings.CameraSensValue, _gameData.MinSensValue, 1f); 
    private float _walkZoom;
    
    // Будет менеджер кидать чей ход точку
    private float _zoomBeforeGame;
    
    // [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private SettingsManager _settings;
    [Inject] private GameData _gameData;

    [InjectOptional] private IOrbitalRotationInput _orbitalRotationInput;
    [Inject] private IDeviceTypeProvider _deviceType;
    [Inject] private InputType _inputType;
    [Inject] private IInputActivity _inputActivity;
    [Inject] private BattleManager _battleManager;
    [Inject] private ThrowGameStarter _gameStarter;


    private void OnEnable()  {
        _settings.CameraZoomChanged += ChangeCameraZoomPercent;
        SystemEvents.WindowOpened += ForbidRotate;
        SystemEvents.ForbidZoomChanged += ForbidZoom;
        _gameStarter.GameStarted += OnGameStarted;
        GameEvents.ShakeCamera += ShakeCamera;
    }

    private void OnPlayerHited() {
        // if (_battleManager.MainPlayerPlay) {
        //     ShakeCamera();
        // }
    }


    private void Update() {
        _rotationHandler.Invoke();
    }

    public void WatchToPoint(Transform point) {
        Debug.Log("WatchToPoint " + point.position);
        _zoomBeforeGame = CurrentFovPercent;
        SetFollowPoint(point);
        SetDamping(_dampningInWinnerWindow);
    }
    
    public void ShakeCamera() {
        if (_noise == null) return;
        _noise.enabled = true;
        _noise.AmplitudeGain = _intensity;
        _noise.FrequencyGain = _frequency;
        
        // Останавливаем тряску через duration секунд
        CancelInvoke(nameof(StopShake));
        Invoke(nameof(StopShake), _duration);
    }
    
    
    public void SetLeftPlayerWinnerAxises(bool leftWinner) {
        if (leftWinner) {
            SetAxisToFollow(_gameData.LeftPlayerWinnerAxis.From, _gameData.LeftPlayerWinnerAxis.To);
        }
        else {
            SetAxisToFollow(_gameData.RightPlayerWinnerAxis.From, _gameData.RightPlayerWinnerAxis.To);
        }
        ChangeCameraZoomPercent(_gameData.ZoomToWinnerView);
    }
    
    public void GoToWinner(Transform point) {
        SetFollowPoint(point);
        SetDamping(_dampningInWinnerWindow);
    }

    public void ResetCameraBeforePlay() {
        ForbidRotate(false);
        ForbidZoom(false);
        SetFollowPoint(_walkPoint);
        ChangeCameraZoomPercent(_zoomBeforeGame);
    }

    private void StopShake() {
        if (_noise == null) return;
        _noise.AmplitudeGain = 0f;
        _noise.FrequencyGain = 0f;
        _noise.enabled = false;
    }
    
    
    private void OnGameStarted(bool started) {
        if(!_battleManager.MainPlayerPlay) return;
        
        // Запрещалкинсы
        ForbidRotate(started);
        ForbidZoom(started);

        // Длительность подлетикса
        SetDamping(started ? _dampningWhlePlay : Vector3.zero);
    }

    private void SetDamping(Vector3 newDamping) {
        _orbitalFollow.TrackerSettings.PositionDamping = newDamping;
    }


    public void SetCameraToPlayThrow(Transform point) {
        _zoomBeforeGame = CurrentFovPercent;
        SetFollowPoint(point);
        SetAxisToFollow(_gameData.HorizontalAxisValueToThrow, _gameData.VerticalAxisValueToThrow);
        ChangeCameraZoomPercent(_gameData.ZoomPercentInThrowGame);
    }
    

    private void SetAxisToFollow(float horizontal, float vertical) {
        _orbitalFollow.HorizontalAxis.Value = horizontal;
        _orbitalFollow.VerticalAxis.Value = vertical;
    }
    

    private void SetFollowPoint(Transform target) {
        _cinemachineCamera.Follow = target;
    }
    
    private void Start() {
        ChangeCameraZoomPercent(_settings.CameraZoomValue);
        
        _orbitalFollow.TrackerSettings.PositionDamping = Vector3.zero;
        
        _walkZoom = CurrentFovPercent;
        if (IsMobile)
            _rotationHandler = HandleJoystickOrbit;
        else {
            // Получаем ссылку на мышь
            _mouse = Mouse.current;
            _rotationHandler = HandleMouseOrbit;
        }
        _defaultX = _orbitalFollow.HorizontalAxis.Value;
        _defaultY = _orbitalFollow.VerticalAxis.Value;
    }

    private void ChangeCameraZoomPercent(float percent) {
        float zoomValue = Mathf.Lerp(_gameData.ZoomDiapasone.From, _gameData.ZoomDiapasone.To, percent);
        ChangeZoom(zoomValue);
    }


    private void SetPoint(Transform point) {
        _cinemachineCamera.Follow = point;
    }


    private void SetDefaultRotation() {
        _orbitalFollow.HorizontalAxis.Value = _defaultX;
        _orbitalFollow.VerticalAxis.Value = _defaultY; 
    }
    


    private void HandleMouseOrbit() {
        // Проверяем нажатие правой кнопки мыши
        if (_mouse.rightButton.wasPressedThisFrame) {
            StartOrbiting();
        }
        else if (_mouse.rightButton.wasReleasedThisFrame) {
            StopOrbiting();
        }
        
        // Вращение
        if (_isOrbiting && _allowRotation) {
            OrbitCamera();
        }
        
        HandleZoom();
    }

    private void StartOrbiting() {
        _isOrbiting = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void StopOrbiting()
    {
        _isOrbiting = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    
    private void HandleJoystickOrbit() {
        if (!_allowRotation) return;

        Vector2 input = _orbitalRotationInput.OrbitalDirection;

        if (input.sqrMagnitude < 0.001f) 
            return;

        float joyX = input.x * Sensitivity * _gameData.JoystickSensivityMultiplier * Time.deltaTime;
        float joyY = input.y * Sensitivity * _gameData.JoystickSensivityMultiplier * Time.deltaTime;

        _orbitalFollow.HorizontalAxis.Value += joyX;
        _orbitalFollow.VerticalAxis.Value -= joyY;

        _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
            _orbitalFollow.VerticalAxis.Value,
            _orbitalFollow.VerticalAxis.Range.x,
            _orbitalFollow.VerticalAxis.Range.y
        );
    }
    
    
    private void OrbitCamera() {
        // Читаем дельту движения мыши
        Vector2 delta = _mouse.delta.ReadValue();
        
        // Применяем чувствительность
        float mouseX = delta.x * Sensitivity * _gameData.MouseSensivityMultiplier;
        float mouseY = delta.y * Sensitivity * _gameData.MouseSensivityMultiplier;
        
        // Вращаем камеру
        _orbitalFollow.HorizontalAxis.Value += mouseX;
        _orbitalFollow.VerticalAxis.Value -= mouseY; // Инвертируем Y
        
 
        // Ограничения
        _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
            _orbitalFollow.VerticalAxis.Value,
            _orbitalFollow.VerticalAxis.Range.x,
            _orbitalFollow.VerticalAxis.Range.y
        );
    }


    private void HandleZoom() {
        float scroll = _mouse.scroll.ReadValue().y * _gameData.ZoomSpeed;
        float zoomValue = _orbitalFollow.RadialAxis.Value - scroll;
        if (Mathf.Abs(scroll) > 0.001f) {
            ChangeZoom(zoomValue);
        }
    }

    
    private void ChangeZoom(float zoomValue) {
        if(!_allowZoom) return;
        _orbitalFollow.RadialAxis.Value = Mathf.Clamp(
            zoomValue,
            _gameData.ZoomDiapasone.From, 
            _gameData.ZoomDiapasone.To
        );
        if (_settings.SettingsIsOpen) {
            _settings.ChangeCameraZoomSilent();
        }
        else {
            if (_waitCameraSave != null) {
                StopCoroutine(_waitCameraSave);
            }   
            _waitCameraSave = StartCoroutine(WaitCameraSave());
        }
    }
    private Coroutine _waitCameraSave;
    private IEnumerator WaitCameraSave() {
        yield return new WaitForSeconds(_cameraSaveDelay);
        _settings.ChangeCameraZoomSilent();
    }
    
    
    private void ForbidZoom(bool forbid) {
        _allowZoom = !forbid;
    }

    private void ForbidRotate(bool windowIsOpen) {
        _allowRotation = !windowIsOpen;
    }
}