using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;

public class BonusShopView : MonoBehaviour {
    [SerializeField] private DelayedTrigger _trgger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _closeButton;
    [Header("Карточки бонусов")]
    [SerializeField] private Transform _healthCard;
    [SerializeField] private Transform _shieldCard;
    [SerializeField] private Transform _resetCard;
    
    
    private void OnEnable() {
        _closeButton.onClick.AddListener(CloseCanvas);
    }


    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trgger.DelayedTriggerAction(OpenBonusCanvasAnimation);
    }

    private void OpenBonusCanvasAnimation() {
        _canvas.ActiveSelf();
    }

    private void CloseCanvas() {
        _canvas.DisactiveSelf();
    }
    
}
