using UnityEngine;
using Zenject;

public class EconomyCalculator : MonoBehaviour {
    // Прокидываем вражеский
    private HitPositionCalculator _enemyHitCalculator;

    
    
    [Inject] BattleManager _battleManager;
    [Inject] ThrowGameStarter _gameStarter;
    
    
    private void OnEnable() {
        _gameStarter.GameStarted += GameStarterOnGameStarted;
    }

    private void GameStarterOnGameStarted(bool started) {
        // Показ расчитанной суммы вызовет потом GameOver
        if (!started) return;
        if(!_battleManager.IsPvbMode) return;
        
    }


    public void SetNewPercentage() {
        
    }
}
