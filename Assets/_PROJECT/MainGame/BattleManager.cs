using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
// Битва, смена ходов, уведомления о ходах которая будет слухать камера!
public class BattleManager : MonoBehaviour {
    // БУдет выбираться рулеткой шо кинуть может
    [SerializeField] private ThrowableObject _labuba;
    [SerializeField] private int PlayersLifeCount;
    
    // Игрок
    [SerializeField] private ObjectThrower _mainPlayer;
    private ObjectThrower _secondPlayer;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] BotsMainManager _botsMainManager;
    [Inject] ObjectThrowerCalculator _throwerCalculator;
    [Inject] CameraOrbitalController _camera;
    
    
    
    private void OnEnable() {
        _throwerCalculator.ObjectThrowed += FocusCamera;
        _throwerCalculator.ObjectFalled += OnObjectFalled;
    }

    private void OnObjectFalled() {
        _stepIsOver = true;
    }

    private void FocusCamera(Transform obj) {
        _camera.ChangeCameraFollow(obj);
    }

    
    
    /// <summary>
    /// Поидее надо будет выбирать режим PVP PVB
    /// </summary>
    public void InitForNewGame() {
        // Настройка первого игрока
        Transform playerPoint = _throwerCalculator.LeftPoint;
        _playerMovement.TpPlayerInPoint(playerPoint);
        _playerMovement.RotateToTarget(_throwerCalculator.RightPoint.position);
        
        // Настройка второго игрока
        Transform secondPlayerPoint = _throwerCalculator.RightPoint;
        BotStateManager secondPlayer = _botsMainManager.GetRandomBotToBattle();
        secondPlayer.TpInPoint(secondPlayerPoint.position);
        secondPlayer.RotateToTarget(_throwerCalculator.LeftPoint.position);
        
        
        _secondPlayer = secondPlayer.BotThrower;
        GoBattle().Forget();
    }

    private bool _stepIsOver;
    private async UniTask GoBattle() {
        // Пока без хп 10 ходов чисто
        for (int i = 0; i < 10; i++) {
            _mainPlayer.SetAllowToThrow(true);
            _stepIsOver = false;
            FocusCamera(_mainPlayer.PointToCameraFocus);
            await UniTask.WaitUntil(() => _stepIsOver);
            _mainPlayer.SetAllowToThrow(false);
            // както-то закончить ход игрока
            
            
            _secondPlayer.SetAllowToThrow(true);
            _stepIsOver = false;
            FocusCamera(_secondPlayer.PointToCameraFocus);
            await UniTask.WaitUntil(() => _stepIsOver);
            _secondPlayer.SetAllowToThrow(false);
        }
    }


    public void DoStep(ObjectThrower thrower) {
        Transform enemyPoint =  
            thrower.ThrowPoint == _mainPlayer.ThrowPoint 
            ? _secondPlayer.transform 
            :
            _mainPlayer.transform;
        
        _throwerCalculator.ThrowNewObject(thrower.AngleToThrow, _labuba, thrower.ThrowPoint, enemyPoint);
        _stepIsOver = false;
    }
    
    
}