using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    
    [Inject(Id = "CanvasesToHide")] private GameObject[] _сanvasesToHide;
    [Inject] IInputActivity _inputActivity;
    
    
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        
        
        _сanvasesToHide.DisactiveSelf();
        
        foreach (var obj in _сanvasesToHide) {
            if (obj == null) continue;

            if (obj.activeSelf) {
                obj.SetActive(false);
            }
        }
        
        
        _inputActivity.Disable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        foreach (var obj in _сanvasesToHide) {
            if (obj == null) continue;

            if (!obj.activeSelf) {
                obj.SetActive(true);
            }
        }
        SystemEvents.WindowOpen(false);
        _inputActivity.Enable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
    
    
}