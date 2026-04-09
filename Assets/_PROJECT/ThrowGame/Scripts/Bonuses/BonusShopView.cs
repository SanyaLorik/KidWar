using System.Collections.Generic;
using Architecture_M;
using DG.Tweening;
using MirraSDK_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class BonusShopView : MonoBehaviour {
    [SerializeField] private DelayedTrigger _trigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _closeButton;
    [Header("Карточки бонусов")]
    [SerializeField] private Ease _easeToShowCards;
    [SerializeField] private float _showCardsDuration;
    [Header("Кнопки купить")]
    [SerializeField] private List<BonusCardItem> _bonusCards;
    [SerializeField] private Button _randomByAdv;

    private GameSave Saves => _save.GetSave<GameSave>();
    
    [Inject] private BonusManager _bonusManager;
    [Inject] private IGameSave _save;
    [Inject] private AdvTimerStarter _advTimerStarter;
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetization;
    [Inject] private PlayerBank _bank;

    
    private void OnEnable() {
        _closeButton.onClick.AddListener(CloseCanvas);
        _bonusCards.ForEach(c => c.BuyButton.onClick.AddListener(() => BuyOneItem(c.Bonus.Id, c)));
        _randomByAdv.onClick.AddListener(WatchAdv);
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
    
    
    private void OnOpenCanvas() {
        _bonusCards.ForEach(c => c.SetCount(Saves.GetBonusCount(c.Bonus.Id)));
        _bonusCards.ForEach(c => c.CheckPlayerBankToBuy());
    }
    

    private void BuyOneItem(string bonusId, BonusCardItem bonusCard) {
        _bank.SpendMoney(bonusCard.Bonus.Price);
        Saves.AddNewBonusCounts(bonusId,1);
        _save.Save();
        bonusCard.SetCount(Saves.GetBonusCount(bonusId));
        _bonusCards.ForEach(c => c.CheckPlayerBankToBuy());
    }

    
    private void WatchAdv() {
        _advertisingMonetization.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    GetRandom();
                }
            }
        );
    }
    
    
    private void GetRandom() { 
        BonusCardItem bonusCardItem = _bonusCards.GetRandomElement();
        Saves.AddNewBonusCounts(bonusCardItem.Bonus.Id, 1);
        _save.Save();
        bonusCardItem.SetCount(Saves.GetBonusCount(bonusCardItem.Bonus.Id));
    }

    
    private void OpenBonusCanvasAnimation() {
        OnOpenCanvas();
        _canvas.ActiveSelf();
        _bonusCards.ForEach(c => c.Card.localScale = Vector3.zero);
        
        Sequence sequence = DOTween.Sequence();
        foreach (var card in _bonusCards) {
            sequence.Append(
                card.Card
                    .DOScale(1f, _showCardsDuration)
                    .SetEase(_easeToShowCards)
            );
        }
    }


    private void CloseCanvas() {
        _canvas.DisactiveSelf();
    }
    
}
