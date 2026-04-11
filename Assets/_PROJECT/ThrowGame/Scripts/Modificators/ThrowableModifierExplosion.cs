using System;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ThrowableModifierExplosion : IThrowableModifier {
    [SerializeField] private float _percentToExpode = 0.9f; 
    [SerializeField] private float exposionDuration = 1f;
    [SerializeField] private float _scaling = 6f;
    [SerializeField] private int _extraDamage;
    [SerializeField] private float _explosionSize = 2f;

    private bool _isExploded; 
    
    public ThrowableObject ThrowableObject { get; private set; }
    
    public int ExtraDamage => _extraDamage;

    
    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        _isExploded = false;
        ThrowableObject.SetIgnoreOtherColliders();
        ThrowableObject.ExplosionAnimation.Size = _explosionSize;
    }

    
    private void Explode() {
        if (_isExploded) return;
        _isExploded = true;
        ThrowableObject.Animation.Kill();
        ThrowableObject.ExplosionAnimation.Play();
        ThrowableObject.HideModel();
        ThrowableObject.transform.localScale = Vector3.one * _scaling;
        GameEvents.ObjectExplodeInvoke();
    }

    
    public void OnPlayerContact() {
        Explode();
    }
    
    
    public void CalculatePose(float elapsedTime) {
        // Обычыный полет
        float progress = elapsedTime / ThrowableObject.FlightDuration;
        if (progress >= _percentToExpode && !_isExploded) {
            Explode();
        }
        else if (!_isExploded){
            Vector3 newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos, progress);
            float currentHeight = ThrowableObject.Height * ThrowableObject.ThrowCurve.Evaluate(progress);
            newPos.y += currentHeight;
            // ThrowableObject.transform.position = newPos;
            ThrowableObject.Rb.MovePosition(newPos);
            
        }
    }
}