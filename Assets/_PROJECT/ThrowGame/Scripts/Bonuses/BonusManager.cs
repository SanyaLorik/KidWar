using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class BonusManager : MonoBehaviour {
    [SerializeField] private List<BonusChanger> _leftBonusChangers;
    [SerializeField] private List<BonusChanger> _rightBonusChangers;
    [SerializeField] private List<ItemValueBase<IBonus>> _bonusValues;
    [Range(0,1), SerializeField] private float _chanseToTryAgainFindBonusBot;

    private float _totalWeight;
    private BonusChanger _choosedModifierChanger;
    
    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BattleManager _battleManager;
    [Inject] private GameData _data;
    [Inject] private BonusesLoader _bonusesLoader;


    private void OnEnable() {
        _battleManager.NewPlayerTurn += OnNewPlayerTurn;
        _battleManager.NewPlayerTurn += OnNewPlayerTurn;
    }
    
    private void Start() {
        CalculateValueDivider();
        _bonusesLoader.LoadBonusesComponents(_leftBonusChangers, _rightBonusChangers);
    }

    private void CalculateValueDivider() {
        _totalWeight = _bonusValues.Sum(m => m.Weight);
    }

    private void OnNewPlayerTurn() {
        _leftBonusChangers.ForEach(b => b.CheckAvailable());
        _rightBonusChangers.ForEach(b => b.CheckAvailable());
    }

    public void UseBonusByClick(IBonus bonus, BonusChanger bonusChanger) {
        if(_battleManager.MainPlayerPlay && _battleManager.BotTurnNow) return;
        TryUseBonus(bonus, bonusChanger);
    }

    private void TryUseBonus(IBonus bonus, BonusChanger bonusChanger) {
        if(!_battleManager.AllowToPlay) return;
        Debug.Log("_battleManager.IsFirstThrowerStep = " + _battleManager.IsFirstThrowerStep);
        Debug.Log("IsLeftPlayerBonus(bonusChanger) = " + IsLeftPlayerBonus(bonusChanger));
        //  Вызвал свой бонус в свой ход левый игрок
        if (IsLeftPlayerBonus(bonusChanger)) {
            if (_battleManager.IsFirstThrowerStep == true) {
                bonus.Use(_battleManager.FirstThrower.ObjectThrower.Damageable);
                bonusChanger.GetOneBonus(_battleManager.IsPvbMode);
                _leftBonusChangers.ForEach(b => b.SetUnvailable());
            }
        }
        //  Вызвал свой бонус в свой ход правый игрок
        else {
            if (_battleManager.IsFirstThrowerStep == false) {
                bonus.Use(_battleManager.SecondThrower.ObjectThrower.Damageable);
                bonusChanger.GetOneBonus();
                bonusChanger.SetUnvailable();
                _rightBonusChangers.ForEach(b => b.SetUnvailable());
            }
        }

    }
    
    public void UseBonusForBot() {
        List<BonusChanger> bonusesChangersList = _battleManager.IsFirstThrowerStep ? 
            _leftBonusChangers 
            : 
            _rightBonusChangers;
        
        // Фаза 1
        if (TryUseRandomBonusForBot(bonusesChangersList)) return;
        if(Random.value > _chanseToTryAgainFindBonusBot) return;
        TryUseRandomBonusForBot(bonusesChangersList);
    }

    private bool TryUseRandomBonusForBot(List<BonusChanger> bonusesChangersList) {
        IBonus bonus = ItemValueBase.GetRandomItemByWeight(_bonusValues, _totalWeight);
        BonusChanger bonusChanger = bonusesChangersList.Find(b => b.Bonus.GetType() == bonus.GetType());
        if (bonusChanger.BonusCount != 0) {
            if (bonus is HealBonus && _battleManager.GetCurrentPlayerLifesCount() == _data.PlayerMaxHp) {
                return false;
            }
            if (bonus is ShieldBonus &&_battleManager.CheckCurrentPlayerUseShield()) {
                return false;
            }
            // use 3rd modifier...
        }
        else {
            return false;
        }
        Debug.Log($"Бот юзает бонус: {bonus.GetType()}, _battleManager.BotTurnNow = {_battleManager.BotTurnNow}");
        TryUseBonus(bonusChanger.Bonus, bonusChanger);
        return true;
    }


    private bool IsLeftPlayerBonus(BonusChanger bonusChanger) {
        return _leftBonusChangers.FindIndex(mc => mc == bonusChanger) != -1;
    }
}