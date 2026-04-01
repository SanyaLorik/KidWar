using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;



[Serializable]
public class ThrowableModifierGigant : IThrowableModifier {
    [SerializeField] private float _scaling = 4f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Ease _ease = Ease.OutBounce;
    public ThrowableObject ThrowableObject { get; private set; }

    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        ThrowableObject.SetIgnoreOtherColliders();
        GiantBeingAsync().Forget();
    }


    private async UniTask GiantBeingAsync() {
        await UniTask.WaitForSeconds(1f);
        ThrowableObject.transform.DOScale(_scaling, _duration)
            .SetEase(_ease)
            .SetUpdate(UpdateType.Fixed);
    }

    public void OnPlayerContact() { }


    public void CalculatePose(float elapsedTime) {
        // Обычыный полет
        float progress = elapsedTime / ThrowableObject.FlightDuration;
        
        Vector3 newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos, progress);
        float currentHeight = ThrowableObject.Height * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        // ThrowableObject.transform.position = newPos;
        ThrowableObject.Rb.MovePosition(newPos);
        
    }
}