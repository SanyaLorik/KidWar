using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
// Битва, смена ходов, уведомления о ходах которая будет слухать камера!


public class BattleManager : MonoBehaviour {
    // Будет выбираться рулеткой шо кинуть может
    [SerializeField] private ThrowableObject _labuba;
    [SerializeField] private int _secondsInStep;
    [SerializeField] private BotObjectThrower _bot1;
    [SerializeField] private BotObjectThrower _bot2;
    
    [SerializeField] private Transform _rightCameraFocus;
    [SerializeField] private Transform _leftCameraFocus;
    
    public IThrowGamePlayer FirstThrower { get; private set; }
    public IThrowGamePlayer SecondThrower { get; private set; }
    
    
    private bool _stepIsOver;
    private bool GameIsOver => FirstThrower.ObjectThrower.CurrentLifesCount == 0 ||  SecondThrower.ObjectThrower.CurrentLifesCount == 0;
    public bool IsFirstThrowerStep { get; private set; }
    public bool BotTurnNow { get; private set; }
    
    public event Action NewPlayerTurn;

    
    [Inject] IThrowGamePlayer _mainPlayer;
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
        Debug.Log("Focus camera");
        _camera.SetCameraToPlayThrow(obj);
    }
    
    
    // Надо будет сделать скрытного бота который будет дублировать внешность игрока
    // и выступать за 2го игрока, покачто это бот с управлением игрока
    /// <summary>
    /// Поидее надо будет выбирать режим PVP PVB
    /// </summary>
    public void InitForNewGame(bool firstPlayerBot, bool secondPlayerBot) {
        Debug.Log("firstPlayerBot " + firstPlayerBot);
        Debug.Log("secondPlayerBot " + secondPlayerBot);
  
        if (!firstPlayerBot) {
            FirstThrower = _mainPlayer;
            GetReadyPlayer(_mainPlayer, true);
        }
        else {
            FirstThrower = _botsMainManager.GetRandomBotToBattle();
            GetReadyPlayer(FirstThrower, true);
        }

        // Второй игрок всегда ботяра, ток с настроенным поведением
        SecondThrower = _botsMainManager.GetRandomBotToBattle();
        GetReadyPlayer(SecondThrower, false);
        
        
        FirstThrower.ObjectThrower.InitToNewGame(true);
        FirstThrower.ObjectThrower.SetBotBehaviour(_bot1, SecondThrower.ObjectThrower, firstPlayerBot);
        
        SecondThrower.ObjectThrower.InitToNewGame(false);
        SecondThrower.ObjectThrower.SetBotBehaviour(_bot2, FirstThrower.ObjectThrower, secondPlayerBot);
        SecondThrower.ObjectThrower.SetBotBehaviour(_bot2, FirstThrower.ObjectThrower, secondPlayerBot);
        
        GoBattle().Forget();
    }

    private void GetReadyPlayer(IThrowGamePlayer player, bool inLeft) {
        if (inLeft) {
            player.TpInPoint(_throwerCalculator.LeftPoint.position);
            player.RotateToTarget(_throwerCalculator.RightPoint.position);
        }
        else {
            player.TpInPoint(_throwerCalculator.RightPoint.position);
            player.RotateToTarget(_throwerCalculator.LeftPoint.position);
        }
        player.SetPlayStatus(true);
    }
    


    private async UniTask GoBattle() {
        Debug.Log("GoBattle");
        
        while(!GameIsOver) {
            IsFirstThrowerStep = true;
            await PlayerStepAsync(FirstThrower.ObjectThrower, _leftCameraFocus);
            IsFirstThrowerStep = false;
            await PlayerStepAsync(SecondThrower.ObjectThrower, _rightCameraFocus);
        }

        SetSpawnState();
        _throwGameStarter.GameOver();
        Debug.Log("Игра закончилась !");
    }

    
    private async UniTask PlayerStepAsync(ObjectThrower thrower, Transform pointToCameraFocus) {
        Debug.Log("NewPlayerTurn");
        NewPlayerTurn?.Invoke();
        _stepIsOver = false;
        thrower.SetAllowToThrow(true);
        if (!thrower.PlayerHandle) {
            BotTurnNow = true;
        }
        
        _timerToThrowStep.StartTimer(_secondsInStep);
        _windChooser.UpdateWind();
        FocusCamera(pointToCameraFocus);
        
        
        thrower.Damageable.SetInvinsible(true);
        await UniTask.WaitUntil(() => _stepIsOver || GameIsOver);
        thrower.Damageable.SetInvinsible(false);
        
        
        thrower.SetAllowToThrow(false);
        if (!thrower.PlayerHandle) {
            BotTurnNow = false;
        }
    }


    private void SetSpawnState() {
        Debug.Log("SetSpawnState");


        if (SecondThrower != null) {
            _camera.ResetCameraBeforePlay();
            SecondThrower.ObjectThrower.SetAllowToThrow(false);
            FirstThrower.ObjectThrower.SetAllowToThrow(false);
        }

        if (FirstThrower != null && FirstThrower.IsPlaying) {
            FirstThrower.SetPlayStatus(false);
            
        }

        if (SecondThrower != null && SecondThrower.IsPlaying) {
            SecondThrower.SetPlayStatus(false);
        }
    }

    
    public void DoStep(ObjectThrower thrower) {
        thrower.SetAllowToThrow(false);
        _timerToThrowStep.StopTimer();
        Vector3 enemyPoint =  
            thrower.ThrowPoint == FirstThrower.ObjectThrower.ThrowPoint 
            ? SecondThrower.ObjectThrower.PointToBeat.position 
            :
            FirstThrower.ObjectThrower.PointToBeat.position;

        // Чтоб не в ноги летело а в тело
        enemyPoint.y += 1f;
        
        _throwerCalculator.ThrowNewObject(thrower.AngleToThrow, _labuba, thrower.ThrowPoint, enemyPoint);
        _stepIsOver = false;
    }
    
    
}