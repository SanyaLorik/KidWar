using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    
    [Inject(Id = "CanvasToHide")] private GameObject _сanvasToHide;
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        _сanvasToHide.DisactiveSelf();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        _сanvasToHide.ActiveSelf();
        SystemEvents.WindowOpen(false);
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}