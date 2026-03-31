using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BonusManager : MonoBehaviour {
    [SerializeField] private List<BonusChanger> _leftBonusChangers;
    [SerializeField] private List<BonusChanger> _rightBonusChangers;

    private BonusChanger _choosedModifierChanger;

    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BattleManager _battleManager;


    private void OnEnable() {
        _battleManager.NewPlayerTurn += OnNewPlayerTurn;
    }

    private void OnNewPlayerTurn() {
        _leftBonusChangers.ForEach(b => b.CheckAvailable());
        _rightBonusChangers.ForEach(b => b.CheckAvailable());
    }

    public void TryUseBonus(IBonus bonus, BonusChanger bonusChanger) {
        //  Вызвал свой бонус в свой ход левый игрок
        if (IsLeftPlayerBonus(bonusChanger)) {
            if (_battleManager.IsFirstThrowerStep == true) {
                bonus.Use(_battleManager.FirstThrower.ObjectThrower.Damageable);
                bonusChanger.GetOneBonus();
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
    
    
    private bool IsLeftPlayerBonus(BonusChanger bonusChanger) {
        return _leftBonusChangers.FindIndex(mc => mc == bonusChanger) != -1;
    }
}
