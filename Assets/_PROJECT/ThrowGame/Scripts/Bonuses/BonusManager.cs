using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class BonusManager : MonoBehaviour {
    [SerializeField] private List<BonusChanger> _leftBonusChangers;
    [SerializeField] private List<BonusChanger> _rightBonusChangers;
    [SerializeField] private List<ItemValueBase<IBonus>> _bonusValues;
    [SerializeField] private GameObject[] _allBonuseObjects;
    [Range(0,1), SerializeField] private float _chanseToTryAgainFindBonusBot;

    private float _totalWeight;
    private BonusChanger _choosedModifierChanger;

    private bool _showBonuses = true;

    
    private GameSave _saves => _saver.GetSave<GameSave>();

    [Inject] private IGameSave _saver; 
    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BattleManager _battleManager;
    [Inject] private GameData _data;
    [Inject] private BonusesLoader _bonusesLoader;
    [Inject] private TutorialManager _tutorialManager;


    public event Action<IBonus> BonusUsed; 
    private void OnEnable() {
        _battleManager.NewPlayerTurn += OnNewPlayerTurn;
    }
        

    private void Start() {
        CalculateValueDivider();
        _bonusesLoader.LoadBonusesComponents(_leftBonusChangers, _rightBonusChangers);
    }

    /// <summary>
    /// Загружает TutorialManager при старте в игру
    /// </summary>
    /// <param name="count">Кол-во каждого бонуса во время туториала</param>
    public void InitBonusesCount(int count) {
        // clear - tru, чтоб игрок не нафармил перезаходами в игру бонусов
        foreach (var bonusChanger in _leftBonusChangers) {
            _saves.AddNewBonusCounts(bonusChanger.BonusItem.Id, count, true);
        }
    }
    
    public void SetBonusesEnable(bool state) {
        _allBonuseObjects.ForEach(b => b.gameObject.SetActive(state));
        _showBonuses = state;
    }

    public void SetPlayerEnableOnlyHeal() {
        _rightBonusChangers.ForEach(b => b.SetUnvailable());
        foreach (var leftBonuse in _leftBonusChangers) {
            if (leftBonuse.Bonus is HealBonus) {
                leftBonuse.SetAvailable();
            }
            else {
                leftBonuse.SetUnvailable();
            }
        }
    }
    
    private void CalculateValueDivider() {
        _totalWeight = _bonusValues.Sum(m => m.Weight);
    }
    
    private void OnNewPlayerTurn() {
        if(!_battleManager.MainPlayerPlay) return;
        if(!_showBonuses) return;
        
        if (_battleManager.IsFirstThrowerStep) { 
            _leftBonusChangers.ForEach(b => b.SetAvailable());
            _rightBonusChangers.ForEach(b => b.SetUnvailable());
        }
        else {
            _rightBonusChangers.ForEach(b => b.SetAvailable());
            _leftBonusChangers.ForEach(b => b.SetUnvailable());
        }
        _leftBonusChangers.ForEach(b => b.CheckAvailable());
        _rightBonusChangers.ForEach(b => b.CheckAvailable());
    }

    public void UseBonusByClick(IBonus bonus, BonusChanger bonusChanger) {
        if(_battleManager.MainPlayerPlay && _battleManager.BotTurnNow) return;
        TryUseBonus(bonus, bonusChanger);
    }

    private void TryUseBonus(IBonus bonus, BonusChanger bonusChanger) {
        if(!_battleManager.AllowToPlay) return;
        // Debug.Log("_battleManager.IsFirstThrowerStep = " + _battleManager.IsFirstThrowerStep);
        // Debug.Log("IsLeftPlayerBonus(bonusChanger) = " + IsLeftPlayerBonus(bonusChanger));
        //  Вызвал свой бонус в свой ход левый игрок
        if (IsLeftPlayerBonus(bonusChanger)) {
            if (_battleManager.IsFirstThrowerStep == true) {
                if (bonus is HealBonus && ThrowerHaveFullHp(_battleManager.FirstThrower)) {
                    return;
                }
                bonus.Use(_battleManager.FirstThrower.ObjectThrower.Damageable);
                BonusUsed?.Invoke(bonus);
                bonusChanger.GetOneBonus(_battleManager.IsPvbMode);
                _leftBonusChangers.ForEach(b => b.SetUnvailable());
            }
        }
        //  Вызвал свой бонус в свой ход правый игрок
        else {
            if (_battleManager.IsFirstThrowerStep == false) {
                if (bonus is HealBonus && ThrowerHaveFullHp(_battleManager.SecondThrower)) {
                    return;
                }
                bonus.Use(_battleManager.SecondThrower.ObjectThrower.Damageable);
                BonusUsed?.Invoke(bonus);
                bonusChanger.GetOneBonus();
                bonusChanger.SetUnvailable();
                _rightBonusChangers.ForEach(b => b.SetUnvailable());
            }
        }

    }

    private bool ThrowerHaveFullHp(IThrowGamePlayer thrower) 
        => thrower.ObjectThrower.CurrentLifesCount == _data.PlayerMaxHp;
    

    public void UseBonusForBot() {
        if(!_tutorialManager.TutorialPassed) return;
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
        TryUseBonus(bonusChanger.Bonus, bonusChanger);
        return true;
    }


    private bool IsLeftPlayerBonus(BonusChanger bonusChanger) {
        return _leftBonusChangers.FindIndex(mc => mc == bonusChanger) != -1;
    }
}