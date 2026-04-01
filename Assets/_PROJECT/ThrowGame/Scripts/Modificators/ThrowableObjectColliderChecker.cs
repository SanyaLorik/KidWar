using UnityEngine;

public class ThrowableObjectColliderChecker : MonoBehaviour {
    [SerializeField] private ThrowableObject _throwableObject;
    
    private void OnTriggerEnter(Collider collider) {
        _throwableObject.TriggerCheck(collider);
    }
    
    private void OnCollisionEnter(Collision collision) {
        _throwableObject.CollisionCheck(collision);
    }
}