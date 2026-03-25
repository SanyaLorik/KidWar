using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
// Битва, смена ходов, уведомления о ходах которая будет слухать камера!
public class BattleManager : MonoBehaviour {
    // БУдет выбираться рулеткой шо кинуть может
    [SerializeField] private ThrowableObject _labuba;
    [SerializeField] private TimerToThrowStep _timerToThrowStep;
    [SerializeField] private int _secondsInStep;
    [field: SerializeField] public int PlayersStartHp { get; private set; }

    // Игрок
    [SerializeField] private ObjectThrower _mainPlayer;
    private ObjectThrower _secondPlayerBotThrower;
    private BotStateManager _secondPlayerBotState;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] BotsMainManager _botsMainManager;
    [Inject] ObjectThrowerCalculator _throwerCalculator;
    [Inject] CameraOrbitalController _camera;
    [Inject] ThrowGameStarter _throwGameStarter;

    
    private void OnEnable() {
        _throwerCalculator.ObjectThrowed += FocusCamera;
        // Конец хода если игрок походил чи кончилось время
        _throwerCalculator.ObjectFalled += StepIsOver;
        _timerToThrowStep.TimeIsOver += StepIsOver;
    }

    private void StepIsOver() {
        _stepIsOver = true;
    }


    private void FocusCamera(Transform obj) {
        _camera.SetCameraToPlayThrow(obj);
    }

    
    
    /// <summary>
    /// Поидее надо будет выбирать режим PVP PVB
    /// </summary>
    public void InitForNewGame() {
        // Настройка первого игрока
        _playerMovement.TpPlayerInPoint(_throwerCalculator.LeftPoint.position);
        _playerMovement.RotateToTarget(_throwerCalculator.RightPoint.position);
        
        // Настройка второго игрока
        Transform secondPlayerPoint = _throwerCalculator.RightPoint;
        _secondPlayerBotState = _botsMainManager.GetRandomBotToBattle();
        
        _playerMovement.SetPlayStatus(true);
        _secondPlayerBotState.SetPlayStatus(true);
        
        _secondPlayerBotState.TpInPoint(secondPlayerPoint.position);
        _secondPlayerBotState.RotateToTarget(_throwerCalculator.LeftPoint.position);
        _secondPlayerBotThrower = _secondPlayerBotState.BotThrower;
        
        _mainPlayer.SetStartHp(PlayersStartHp);
        _secondPlayerBotThrower.SetStartHp(PlayersStartHp);
        
        GoBattle().Forget();
    }

    private bool _stepIsOver;
    private bool GameIsOver => _mainPlayer.CurrentLifesCount == 0 ||  _secondPlayerBotThrower.CurrentLifesCount == 0;
    
    
    private async UniTask GoBattle() {
        // Пока без хп 10 ходов чисто
        
        while(!GameIsOver) {
            _mainPlayer.SetAllowToThrow(true);
            _timerToThrowStep.StartTimer(_secondsInStep);
            _stepIsOver = false;
            FocusCamera(_mainPlayer.PointToCameraFocus);
            await UniTask.WaitUntil(() => _stepIsOver || GameIsOver);
            _mainPlayer.SetAllowToThrow(false);
            // както-то закончить ход игрока
            
            
            _secondPlayerBotThrower.SetAllowToThrow(true);
            _timerToThrowStep.StartTimer(_secondsInStep);
            _stepIsOver = false;
            FocusCamera(_secondPlayerBotThrower.PointToCameraFocus);
            await UniTask.WaitUntil(() => _stepIsOver || GameIsOver);
            _secondPlayerBotThrower.SetAllowToThrow(false);
        }

        SetSpawnState();
        Debug.Log("Игра закончилась нахуй!");
    }


    private void SetSpawnState() {
        _throwGameStarter.GameOver();
        
        _secondPlayerBotThrower.SetAllowToThrow(false);
        _mainPlayer.SetAllowToThrow(false);
        _camera.ResetCameraBeforePlay();
        
        _secondPlayerBotState.SetPlayStatus(false);
        _playerMovement.SetPlayStatus(false);
    }

    public void DoStep(ObjectThrower thrower) {
        thrower.SetAllowToThrow(false);
        _timerToThrowStep.StopTimer();
        Transform enemyPoint =  
            thrower.ThrowPoint == _mainPlayer.ThrowPoint 
            ? _secondPlayerBotThrower.transform 
            :
            _mainPlayer.transform;
        
        _throwerCalculator.ThrowNewObject(thrower.AngleToThrow, _labuba, thrower.ThrowPoint, enemyPoint);
        _stepIsOver = false;
    }
    
    
}