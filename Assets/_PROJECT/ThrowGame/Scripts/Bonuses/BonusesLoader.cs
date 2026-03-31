using System;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

/// <summary>
/// Загружает кол-во бонусов игрокам и ботам
/// </summary>
public class BonusesLoader : MonoBehaviour {
    [SerializeField] private int _maxRandomCountToBot;
    [SerializeField] private int _countToPvp;
    
    private List<BonusChanger> _leftBonusChangers;
    private List<BonusChanger> _rightBonusChangers;

    [Inject] private ThrowGameStarter _starter;
    [Inject] private BattleManager _battleManager;
    [Inject] private IGameSave _save;
    
    
    public void LoadBonusesComponents(List<BonusChanger> leftBonusChangers, List<BonusChanger> rightBonusChangers) {
        _leftBonusChangers = leftBonusChangers;
        _rightBonusChangers = rightBonusChangers;
    }

    private void OnEnable() {
        _starter.GameStarted += StarterOnGameStarted;
    }

    
    /// <summary>
    /// 3 случая:
    /// 1. PVP - обоим загружать кол-во равное главному игроку 
    /// 2. PVB - игроку загружать, боту рандомно
    /// 3. BVB - 
    /// </summary>
    /// <param name="state"></param>
    private void StarterOnGameStarted(bool state) {
        if(!state) return;
        if (_battleManager.MainPlayerPlay) {
            // 1 и 2 случай обработка
            // Загрузка бонусов игрока из сейвов...
            // PVP
            if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
                _leftBonusChangers.ForEach(b => b.SetBonusCount(_countToPvp));
                _rightBonusChangers.ForEach(b => b.SetBonusCount(_countToPvp));
            }
            // PVB
            else {
                _leftBonusChangers.ForEach(b => b.SetBonusCount(_save.GetSave<GameSave>().GetBonusCount(b.BonusItem.Id)));
                _rightBonusChangers.ForEach(b => b.SetBonusCount(Random.Range(0, _maxRandomCountToBot)));
            }
        }
        else {
            // 2 бота тут пахую
            _leftBonusChangers.ForEach(b => b.SetBonusCount(Random.Range(0, _maxRandomCountToBot)));
            _rightBonusChangers.ForEach(b => b.SetBonusCount(Random.Range(0, _maxRandomCountToBot)));
        }
        
        
        
    }
}