using Architecture_M;
using UnityEngine;

public class ThrowableObject : MonoBehaviour {
    [SerializeField] private DOTweenAnimationBase _animation;
    [field: SerializeField] public float Force { get; private set; }

    public void ObjectIsFall() {
        _animation.Kill();
    }

    public void ChangeObjectForce(float force) {
        Force =  force;
    }

    public void ChangeObjectSize() {}
}
