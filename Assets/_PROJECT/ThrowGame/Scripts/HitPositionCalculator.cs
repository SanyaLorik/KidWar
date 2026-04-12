using UnityEngine;
using Zenject;

[RequireComponent(typeof(BoxCollider))]
public class HitPositionCalculator : MonoBehaviour {
    private BoxCollider _collider;
    private bool _allowCalculate;

    [Inject] EconomyCalculator _economyCalculator;

    private float _percentage;


    private void Awake() {
        _collider = GetComponent<BoxCollider>();
    }

    public void SetCalculateState(bool calculate) {
        _allowCalculate = calculate;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (!_allowCalculate) return;
        
        ContactPoint contact = collision.GetContact(0);
        _percentage = CalculateHitPercentage(contact.point)/100f;
        // Debug.Log($"Попадание на высоте: {_percentage:F1}%");
        _economyCalculator.AddNewBodyRatio(_percentage);
        
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
