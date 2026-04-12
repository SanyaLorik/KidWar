using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;

public class WindChooseView : MonoBehaviour {
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private RectTransform _progressParent;
    [Header("Диапазон силы ветра")] 
    [SerializeField] private PairedValue<float> _windForce;
    
    [Header("Указатель ветра анимация")] 
    [SerializeField] private float _pointerScaleRatio;
    [SerializeField] private float _pointerScaleDuration;
    [SerializeField] private Ease _pointerUpScaleEase;
    [SerializeField] private Ease _pointerDownScaleEase;
    
    public float CurrentWindForce { get; private set; }

    
    public void UpdateWind() {
        CurrentWindForce = Random.Range(_windForce.From, _windForce.To); 
        // Свести к [-0.5; 0.5]
        float _windPercent = CurrentWindForce / _windForce.To / 2f;
        
        float xEnd = RectTransformHelper.CalculateXEnd(_progressParent);
        SetPointerNegative(_pointer, _windPercent, xEnd);
        AnimatePointer();
        // Debug.Log("установка уэтра! " + CurrentWindForce);
    }

    private void AnimatePointer() {
        Sequence seq = DOTween.Sequence();
        _pointer.localScale = Vector3.one;
        seq.Append(_pointer
            .DOScale(_pointerScaleRatio, _pointerScaleDuration)
            .SetEase(_pointerUpScaleEase)   
        );
        seq.Append(_pointer
            .DOScale(1f, _pointerScaleDuration)
            .SetEase(_pointerDownScaleEase)   
        );
            
    }
    
    
    /// <summary>
    /// Можно процент поставить отрицательный
    /// </summary>
    private static void SetPointerNegative(RectTransform pointer, float percent, float xEnd, float offset = 0)
    {
        Vector2 newPointerPos = new Vector2(xEnd * percent + offset, pointer.anchoredPosition.y);
        pointer.anchoredPosition = newPointerPos;
    }
    
}
