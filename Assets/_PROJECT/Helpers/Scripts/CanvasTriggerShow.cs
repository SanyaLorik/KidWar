using System;
using SanyaBeerExtension;
using UnityEngine;

public class CanvasTriggerShow : MonoBehaviour {
    [SerializeField] private GameObject _canvas;


    private void Awake() {
        _canvas.DisactiveSelf();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _canvas.ActiveSelf();
        }
    }
    
    
    private void OnTriggerExit(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _canvas.DisactiveSelf();
        }
    }
}