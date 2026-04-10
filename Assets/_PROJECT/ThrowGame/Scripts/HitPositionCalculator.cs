using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HitPositionCalculator : MonoBehaviour {
    private BoxCollider _collider;
    private bool _allowCalculate = true;

    public float Percentage { get; private set; }


    private void Awake() => _collider = GetComponent<BoxCollider>();

    public void SetCalculateState(bool calculate) {
        _allowCalculate = calculate;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (!_allowCalculate) return;
        
        ContactPoint contact = collision.GetContact(0);
        Percentage = CalculateHitPercentage(contact.point);
        Debug.Log($"Попадание на высоте: {Percentage:F1}%");
        
        _allowCalculate = false;
    }

    private float CalculateHitPercentage(Vector3 worldHitPoint)
    {
        // Переводим мировую точку в локальную
        Vector3 localPoint = transform.InverseTransformPoint(worldHitPoint);
        
        // Получаем РЕАЛЬНЫЕ границы коллайдера с учётом его center
        float colliderMinY = _collider.center.y - _collider.size.y / 2f;
        float colliderMaxY = _collider.center.y + _collider.size.y / 2f;
        
        // Вычисляем процент (0% = низ, 100% = верх)
        float percentage = Mathf.InverseLerp(colliderMinY, colliderMaxY, localPoint.y) * 100f;
        
        return percentage;
    }
}

public class HitCalculatorEnabler : MonoBehaviour {
    
}
