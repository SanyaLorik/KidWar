using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    
    [Inject(Id = "CanvasToHide")] private GameObject _сanvasToHide;
    [Inject] IInputActivity _inputActivity;
    
    
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        _сanvasToHide.DisactiveSelf();
        _inputActivity.Disable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        _сanvasToHide.ActiveSelf();
        SystemEvents.WindowOpen(false);
        _inputActivity.Enable();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}