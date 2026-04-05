using SanyaBeerExtension;
using UnityEngine;

public class WindChooseView : MonoBehaviour {
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private RectTransform _progressParent;
    [Header("Диапазон силы ветра")] 
    [SerializeField] private PairedValue<float> _windForce;
    
    

    public float CurrentWindForce { get; private set; }

    
    public void UpdateWind() {
        CurrentWindForce = Random.Range(_windForce.From, _windForce.To); 
        // Свести к [-0.5; 0.5]
        float _windPercent = CurrentWindForce / _windForce.To / 2f;
        
        float xEnd = RectTransformHelper.CalculateXEnd(_progressParent);
        SetPointerNegative(_pointer, _windPercent, xEnd);
        // Debug.Log("установка уэтра! " + CurrentWindForce);
    }
    
    
    /// <summary>
    /// Можно процент поставить отрицательный
    /// </summary>
    public static void SetPointerNegative(RectTransform pointer, float percent, float xEnd, float offset = 0)
    {
        Vector2 newPointerPos = new Vector2(xEnd * percent + offset, pointer.anchoredPosition.y);
        pointer.anchoredPosition = newPointerPos;
    }
    
}
