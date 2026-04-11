using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ThrowableModifierDouble : IThrowableModifier {
    [SerializeField] private float _heightMultiplier = 1.3f;
    [SerializeField] private int _extraDamage;
    [SerializeField] private float _secondThrowExtendDistance;

    private ThrowableObject _secondThrowableObject;
    private Vector3 _extendDistance;
    public ThrowableObject ThrowableObject { get; private set; }
    public int ExtraDamage => _extraDamage;


    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        _secondThrowableObject = Object.Instantiate(ThrowableObject);
        _secondThrowableObject.SetDefaultModifier();
        _extendDistance = new Vector3(0,0, _secondThrowExtendDistance);
    }

    public void OnPlayerContact() {
        _secondThrowableObject.StartDestroyTimer();
    }

    
    public void CalculatePose(float elapsedTime) {
        // Обычыный полет
        float progress = elapsedTime / ThrowableObject.FlightDuration;
        
        Vector3 newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos, progress);
        float currentHeight = ThrowableObject.Height * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        // ThrowableObject.transform.position = newPos;
        ThrowableObject.Rb.MovePosition(newPos);
        
        // + полет второго обьекта
        newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos + _extendDistance, progress);
        currentHeight = ThrowableObject.Height * _heightMultiplier * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        // _secondThrowableObject.transform.position = newPos;
        _secondThrowableObject.Rb.MovePosition(newPos);
        
    }

}