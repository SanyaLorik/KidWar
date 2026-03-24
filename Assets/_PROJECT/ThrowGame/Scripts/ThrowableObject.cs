using Architecture_M;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThrowableObject : MonoBehaviour {
    [SerializeField] private DOTweenAnimationBase _animation;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _bounceForce = .5f;
    [SerializeField] private float _rotationForceAfterFall = -5f;
    [field: SerializeField] public int Force { get; private set; }
    
    public bool ContactPlayer { get; private set; } 
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out ObjectThrower thrower) || ContactPlayer) return;
        thrower.MinusHp(Force);
        ContactPlayer = true;
    }

    private void OnCollisionEnter(Collision other) {
        EnablePhysicsAfterHit(other);
        if (!other.gameObject.TryGetComponent(out ObjectThrower thrower)) {
            // Об землю ударилось, сё низя урон наносить
            ContactPlayer = true;
        }
    }

    private void EnablePhysicsAfterHit(Collision collision) {
        // Включаем гравитацию
        _rb.useGravity = true;
        
        // Отталкиваемся от точки столкновения
        ContactPoint contact = collision.contacts[0];
        Vector3 bounceDir = Vector3.Reflect(_rb.linearVelocity, contact.normal);
        bounceDir.y = Mathf.Abs(bounceDir.y) + 2f; // Добавляем отскок вверх

        _rb.linearVelocity = bounceDir * _bounceForce; // Отскок с силой 3
        
        // Добавляем вращение
        _rb.angularVelocity = new Vector3(
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall),
            Random.Range(-_rotationForceAfterFall, _rotationForceAfterFall)
        );
        // Уничтожаем через 3 секунды
    }

    public void SetUseGravity(bool useGravity) {
        
    }
    
    public void ObjectIsFall() {
        _animation.Kill();
    }

    public void ChangeObjectForce(int force) {
        Force =  force;
    }

    public void ChangeObjectSize() {}


    
}
