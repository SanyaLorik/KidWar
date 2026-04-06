using System;
using UnityEngine;

[Serializable]
public class ThrowableModifierAim : IThrowableModifier {
    [SerializeField] private float _percentageToStart = .6f;
    [SerializeField] private int _extraDamage;

    public ThrowableObject ThrowableObject { get; private set; }

    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
        ThrowableObject.ChangeTimeDuration(ThrowableObject.FlightDurationToEnemy);
    }

    public void ExtensionBehaviour() { }
    public int ExtraDamage => _extraDamage;

    public void OnPlayerContact() { }

    
    public void CalculatePose(float elapsedTime) {
        float progress = elapsedTime / ThrowableObject.FlightDurationToEnemy;
        Vector3 newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.EnemyPose, progress);
        float currentHeight = ThrowableObject.Height * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        // ThrowableObject.transform.position = newPos;
        ThrowableObject.Rb.MovePosition(newPos);
    }
}