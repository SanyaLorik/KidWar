using System;
using DG.Tweening;
using UnityEngine;



[Serializable]
public class ThrowableModifierGigant : IThrowableModifier {
    public ThrowableObject ThrowableObject { get; private set; }

    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        ThrowableObject.transform.DOScale(3f, 1f)
            .SetEase(Ease.OutBounce);
    }


    public void CalculatePose(float elapsedTime) {
        // Обычыный полет
        float progress = elapsedTime / ThrowableObject.FlightDuration;
        
        Vector3 newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos, progress);
        float currentHeight = ThrowableObject.Height * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        ThrowableObject.transform.position = newPos;
    }
}