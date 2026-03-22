using UnityEngine;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        SystemEvents.WindowOpen(false);
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}