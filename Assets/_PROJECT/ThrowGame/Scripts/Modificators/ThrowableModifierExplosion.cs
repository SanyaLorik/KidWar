using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class ThrowableModifierExplosion : IThrowableModifier {
    [SerializeField] private float _percentToExpode = 0.9f; 
    [SerializeField] private float exposionDuration = 1f;
    [SerializeField] private float _scaling = 6f;
    [SerializeField] private Ease _ease = Ease.OutBounce;

    private bool _isExploded; 
    
    public ThrowableObject ThrowableObject { get; private set; }
    
    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        _isExploded = false;
        ThrowableObject.SetIgnoreOtherColliders();
    }

    private void Explode() {
        ThrowableObject.transform.DOScale(_scaling, exposionDuration)
            .SetEase(_ease)
            .OnComplete(ThrowableObject.Destroy);
    }

    public void OnPlayerContact() {
        Explode();
    }
    
    
    public void CalculatePose(float elapsedTime) {
        // Обычыный полет
        float progress = elapsedTime / ThrowableObject.FlightDuration;
        if (progress >= _percentToExpode && !_isExploded) {
            _isExploded = true;
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