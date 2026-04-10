using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

[Serializable]
public struct BodyPartReward {
    [Range(0,1), SerializeField] public float BodyPercentage;
    [SerializeField] public float Ratio;
}



public class EconomyCalculator : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private TextMeshProUGUI _doubleRewardText;
    
    
    [SerializeField] private float _ratioForWin;
    [SerializeField] private float _ratioForLoose;
    
    
    [SerializeField] private List<BodyPartReward> _bodyPartRewards;
    
    

    private float _accumulatePercentage;
    private int _rewardMoney;
    
    
    [Inject] PlayerBank _bank;
    [Inject] BattleManager _battleManager;
    [Inject] ThrowGameStarter _gameStarter;
    [Inject] GameOverShower _gameOverShower;
    [Inject] GameData _gameData;
    [Inject] NumberFormatter _formatter;
    
    
    private void OnEnable() {
        _gameStarter.GameStarted += GameStarterOnGameStarted;
        _gameOverShower.PlayerWin += SetResult;
        
    }

    private void SetResult(bool playerWin) {
        if(!_battleManager.IsPvbMode) return;
        
        int damage = _gameData.PlayerMaxHp - _battleManager.SecondThrower.ObjectThrower.CurrentLifesCount;
        Debug.Log($"Игрок выиграл = {playerWin}, урона нанёс {damage}");        
        CalculateRewardAfterBattle(playerWin, damage);
    }

    private void GameStarterOnGameStarted(bool started) {
        // Показ расчитанной суммы вызовет потом GameOver
        if (!started) return;
        if(!_battleManager.IsPvbMode) return;
        // NEW BATTLE
        _accumulatePercentage = 0f;
    }
    
    
    public void AddNewBodyRatio(float percentage) {
        int index = _bodyPartRewards.FindIndex(p => p.Ratio > percentage);
        
        float ratio = index == -1 ? 
            _bodyPartRewards[^1].Ratio 
            :
            _bodyPartRewards[index].Ratio;
        
        Debug.Log($"Попадание в процентаж {percentage*100}, ratio = {ratio}");
        _accumulatePercentage += ratio;
 
        Debug.Log("_accumulatePercentage = " + _accumulatePercentage);
    }

    
    private void CalculateRewardAfterBattle(bool win, int damage) {
        float ratio = win ?  _ratioForWin : _ratioForLoose;
        _rewardMoney = (int)(damage * _accumulatePercentage * ratio);
        Debug.Log("Выигрышь игрока: " + _rewardMoney);
        _rewardText.text = _formatter.ValuteFormatter(_rewardMoney);
        _doubleRewardText.text = _formatter.ValuteFormatter(_rewardMoney*2);
    }
    
    
    public void GetReward(bool doubleReward) {
        int reward = doubleReward ? _rewardMoney * 2 : _rewardMoney;
        _bank.AddMoney(reward);
        _rewardMoney = 0;
    }
    
    
}
