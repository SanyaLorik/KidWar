using System;
using System.Collections.Generic;
using Architecture_M;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[Serializable]
public struct BonusButtonItem {
    public Button BonusButton;
    public BonusItemSO BonuseItem;
}


public class BonusShopView : MonoBehaviour {
    [SerializeField] private DelayedTrigger _trigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _closeButton;
    [Header("Карточки бонусов")]
    [SerializeField] private Transform _healthCard;
    [SerializeField] private Transform _shieldCard;
    [SerializeField] private Transform _resetCard;
    [SerializeField] private Ease _easeToShowCards;
    [SerializeField] private float _showCardsDuration;
    [Header("Кнопки купить")]
    [SerializeField] private List<BonusButtonItem> _bonusButtons;
    [SerializeField] private Button _randomByAdv;
    
    [Inject] private BonusManager _bonusManager;
    [Inject] private IGameSave _save;
    [Inject] private AdvTimerStarter _advTimerStarter;
    
    
    private void OnEnable() {
        _closeButton.onClick.AddListener(CloseCanvas);
        _bonusButtons.ForEach(b => b.BonusButton.onClick.AddListener(() => BuyOneItem(b)));
        _randomByAdv.onClick.AddListener(BuyRandom);
    }

    private void BuyOneItem(BonusButtonItem bonusButtonItem) {
        _save.GetSave<GameSave>().AddNewBonusCounts(bonusButtonItem.BonuseItem.Id,1);
        _save.Save();
    }
    
    private void BuyRandom() { 
        BonusButtonItem bonusButtonItem = _bonusButtons.GetRandomElement();
        _save.GetSave<GameSave>().AddNewBonusCounts(bonusButtonItem.BonuseItem.Id, 1);
        _save.Save();
    }


    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trigger.DelayedTriggerAction(OpenBonusCanvasAnimation);
        _advTimerStarter.DisableTimer();
    }
    
    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trigger.CancelTriggerAction();
        _advTimerStarter.EnableTimer();
    }

    private void OpenBonusCanvasAnimation() {
        _canvas.ActiveSelf();
        _healthCard.localScale = Vector3.zero;
        _shieldCard.localScale = Vector3.zero;
        _resetCard.localScale = Vector3.zero;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            _shieldCard
                .DOScale(1f, _showCardsDuration)
                .SetEase(_easeToShowCards)
        );
        sequence.Append(
            _healthCard
                .DOScale(1f, _showCardsDuration)
                .SetEase(_easeToShowCards)
        );
        sequence.Append(
            _resetCard
                .DOScale(1f, _showCardsDuration)
                .SetEase(_easeToShowCards)
        );

    }

    private void CloseCanvas() {
        _canvas.DisactiveSelf();
    }
    
}
