using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
// Битва, смена ходов, уведомления о ходах которая будет слухать камера!
public class BattleManager : MonoBehaviour {
    // БУдет выбираться рулеткой шо кинуть может
    [SerializeField] private ThrowableObject _labuba;
    [SerializeField] private int _secondsInStep;

    
    // Игроки
    [field: SerializeField] public ObjectThrower MainPlayer { get; private set; }
    public ObjectThrower SecondPlayer { get; private set; }
    
    
    
    private BotStateManager _secondPlayerBotState;
    private bool _stepIsOver;
    private bool GameIsOver => MainPlayer.CurrentLifesCount == 0 ||  SecondPlayer.CurrentLifesCount == 0;
    public bool IsMainPlayerStep { get; private set; }
    public event Action NewPlayerTurn;

    
    
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] BotsMainManager _botsMainManager;
    [Inject] ObjectThrowerCalculator _throwerCalculator;
    [Inject] CameraOrbitalController _camera;
    [Inject] ThrowGameStarter _throwGameStarter;
    [Inject] TimerToThrowStep _timerToThrowStep;
    [Inject] WindChooseView _windChooser;

    
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
        SecondPlayer = _secondPlayerBotState.BotThrower;
        
        MainPlayer.InitToNewGame();
        SecondPlayer.InitToNewGame();
        
        GoBattle().Forget();
    }

    
    
    private async UniTask GoBattle() {
        // Пока без хп 10 ходов чисто
        
        while(!GameIsOver) {
            IsMainPlayerStep = true;
            await PlayerStepAsync(MainPlayer);
            IsMainPlayerStep = false;
            await PlayerStepAsync(SecondPlayer);
        }

        SetSpawnState();
        Debug.Log("Игра закончилась !");
    }

    private async UniTask PlayerStepAsync(ObjectThrower thrower) {
        NewPlayerTurn?.Invoke();
        _stepIsOver = false;
        thrower.SetAllowToThrow(true);
        
        _timerToThrowStep.StartTimer(_secondsInStep);
        _windChooser.UpdateWind();
        FocusCamera(thrower.PointToCameraFocus);
        
        await UniTask.WaitUntil(() => _stepIsOver || GameIsOver);
        
        thrower.SetAllowToThrow(false);
    }


    private void SetSpawnState() {
        _throwGameStarter.GameOver();
        
        SecondPlayer.SetAllowToThrow(false);
        MainPlayer.SetAllowToThrow(false);
        _camera.ResetCameraBeforePlay();
        
        _secondPlayerBotState.SetPlayStatus(false);
        _playerMovement.SetPlayStatus(false);
    }

    public void DoStep(ObjectThrower thrower) {
        thrower.SetAllowToThrow(false);
        _timerToThrowStep.StopTimer();
        Vector3 enemyPoint =  
            thrower.ThrowPoint == MainPlayer.ThrowPoint 
            ? _secondPlayerBotState.transform.position 
            :
            _playerMovement.transform.position;

        // Чтоб не в ноги летело а в тело
        enemyPoint.y += 1f;
        
        _throwerCalculator.ThrowNewObject(thrower.AngleToThrow, _labuba, thrower.ThrowPoint, enemyPoint);
        _stepIsOver = false;
    }
    
    
}