using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ThrowableModifierDouble : IThrowableModifier {
    [SerializeField] private float _heightMultiplier = 1.3f;

    public ThrowableObject ThrowableObject { get; private set; }

    private ThrowableObject _secondThrowableObject;
    public void SetThrowableObject(ThrowableObject throwableObject) {
        ThrowableObject = throwableObject;
    }

    public void ExtensionBehaviour() {
        _secondThrowableObject = Object.Instantiate(ThrowableObject);
        _secondThrowableObject.StartDestroyTimer(true);
    }


    public void OnPlayerContact() {
        _secondThrowableObject.StartDestroyTimer(true);
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
        newPos = Vector3.Lerp(ThrowableObject.InitialPos, ThrowableObject.TargetPos, progress);
        currentHeight = ThrowableObject.Height * _heightMultiplier * ThrowableObject.ThrowCurve.Evaluate(progress);
        newPos.y += currentHeight;
        // _secondThrowableObject.transform.position = newPos;
        _secondThrowableObject.Rb.MovePosition(newPos);
        
    }

    private IEnumerator DeleteAsync() {
        yield return new WaitForSeconds(5f);
        if (_secondThrowableObject != null) {
            Object.Destroy(_secondThrowableObject.gameObject);
        }
    }
}