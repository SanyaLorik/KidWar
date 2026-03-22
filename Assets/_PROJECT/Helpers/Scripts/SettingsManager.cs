using System;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsManager : MonoBehaviour {
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private Button _settingsButtonOpen;
    [SerializeField] private Button _settingsButtonClose;
    
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Slider _cameraZoomSlider;
    [SerializeField] private Slider _cameraSensitivitySlider;
    
    
    private float _musicValue;
    private float _effectsValue;
    private float _cameraZoomValue;
    private float _cameraSensivityValue;
    
    public float MusicValue => _musicValue;
    public float EffectsValue => _effectsValue;
    public float CameraZoomValue => _cameraZoomValue;
    public float CameraSensValue => _cameraSensivityValue;
    
    
    
    private const string MusicKey = "settings_music";
    private const string EffectsKey = "settings_effects";
    private const string CameraZoomKey = "settings_camera_zoom";
    private const string CameraSensKey = "settings_camera_sensitivity";
    
    
    public event Action<float> MusicValueChanged;
    public event Action<float> EffectsValueChanged;
    public event Action<float> CameraZoomChanged;
    
    [Inject] private IInputActivity _inputActivity;
    [Inject] private CameraOrbitalController _camera;
    
    
    private void Awake() {
        _settingsCanvas.DisactiveSelf();
    
        // Подгрузка
        _musicValue = PlayerPrefs.GetFloat(MusicKey, 1f);
        _effectsValue = PlayerPrefs.GetFloat(EffectsKey, 1f);
        _cameraZoomValue = PlayerPrefs.GetFloat(CameraZoomKey, _camera.DefaultFov);
        _cameraSensivityValue = PlayerPrefs.GetFloat(CameraSensKey, _camera.DefaultSens);
    
        // Установка в настройках
        _musicSlider.SetValueWithoutNotify(_musicValue);
        _effectsSlider.SetValueWithoutNotify(_effectsValue);
        _cameraZoomSlider.SetValueWithoutNotify(_cameraZoomValue);
        _cameraSensitivitySlider.SetValueWithoutNotify(_cameraSensivityValue);
    
        
        _settingsButtonOpen.onClick.AddListener(OpenSettings);
        _settingsButtonClose.onClick.AddListener(CloseSettings);
    
        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _effectsSlider.onValueChanged.AddListener(ChangeEffectsVolume);
        
        // Camera
        _cameraZoomSlider.onValueChanged.AddListener(ChangeCameraZoom);
        _cameraSensitivitySlider.onValueChanged.AddListener(ChangeCameraSensivity);
    }
    
    
    private void Start() {
        CameraZoomChanged?.Invoke(_cameraZoomValue);
        EffectsValueChanged?.Invoke(_effectsValue);
        MusicValueChanged?.Invoke(_musicValue);
    }
    
    private void ChangeCameraZoom(float value) {
        _cameraZoomValue = value;
        PlayerPrefs.SetFloat(CameraZoomKey, value);
        CameraZoomChanged?.Invoke(value);
    }
    
    private void ChangeCameraSensivity(float value) {
        _cameraSensivityValue = value;
        PlayerPrefs.SetFloat(CameraSensKey, value);
    }
    
    public void ChangeCameraZoomSilent() {
        PlayerPrefs.SetFloat(CameraZoomKey, _camera.CurrentFovPercent);
        _cameraZoomSlider.SetValueWithoutNotify(_camera.CurrentFovPercent);
        _cameraZoomValue = _camera.CurrentFovPercent;
    }
    
    private void ChangeEffectsVolume(float value) {
        _effectsValue = value;
        PlayerPrefs.SetFloat(EffectsKey, value);
        EffectsValueChanged?.Invoke(value);
    }
    
    
    public bool SettingsIsOpen => _settingsCanvas.activeSelf;
    private void ChangeMusicVolume(float value) {
        _musicValue = value;
        PlayerPrefs.SetFloat(MusicKey, value);
        MusicValueChanged?.Invoke(value);
    }
    
    private void OpenSettings() {
        _settingsCanvas.ActiveSelf();
        _inputActivity.Disable();
        // При открытии оно подгрузит с камеры измененный
        ChangeCameraZoomSilent();
    }

    private void CloseSettings() {
        _settingsCanvas.DisactiveSelf();
        _inputActivity.Enable();
        PlayerPrefs.Save();
    }
}