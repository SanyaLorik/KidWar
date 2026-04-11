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
        _inputActivity.Disable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        _сanvasesToHide.ActiveSelf();
        SystemEvents.WindowOpen(false);
        _inputActivity.Enable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}