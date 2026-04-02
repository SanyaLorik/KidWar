using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
// Битва, смена ходов, уведомления о ходах которая будет слухать камера!


public class BattleManager : MonoBehaviour {
    // Будет выбираться рулеткой шо кинуть может
    [SerializeField] private int _secondsInStep;
    [SerializeField] private BotObjectThrower _bot1;
    [SerializeField] private BotObjectThrower _bot2;
    
    [SerializeField] private Transform _rightCameraFocus;
    [SerializeField] private Transform _leftCameraFocus;
    [SerializeField] private RouletteSkin _roulette;
    
    private ThrowableObject _newThrowableObjectInRoulette;
    
    public IThrowGamePlayer FirstThrower { get; private set; }
    public IThrowGamePlayer SecondThrower { get; private set; }
    
    private bool _stepIsOver;
    private bool GameIsOver => FirstThrower.ObjectThrower.CurrentLifesCount == 0 ||  SecondThrower.ObjectThrower.CurrentLifesCount == 0;
    public bool IsFirstThrowerStep { get; private set; }
    public bool BotTurnNow { get; private set; }
    public bool MainPlayerPlay { get; private set; }

    public bool IsPvbMode => FirstThrower.ObjectThrower.PlayerHandle && !SecondThrower.ObjectThrower.PlayerHandle;

    public bool AllowToPlay { get; private set; }

    public event Action NewPlayerTurn;

    
    [Inject] IThrowGamePlayer _mainPlayer;
    [Inject] BotsMainManager _botsMainManager;
    [Inject] ObjectThrowerCalculator _throwerCalculator;
    [Inject] CameraOrbitalController _camera;
    [Inject] ThrowGameStarter _throwGameStarter;
    [Inject] TimerToThrowStep _timerToThrowStep;
    [Inject] WindChooseView _windChooser;
    [Inject] StartBattleView _startBattleView;
    [Inject] PlayersStepView _playersStepView;
    [Inject] ThrowObjectsIniter _throwObjectsIniter;
    [Inject] GameOverShower _gameOverShower;

    
    public int GetCurrentPlayerLifesCount() {
        if (IsFirstThrowerStep) {
            return FirstThrower.ObjectThrower.CurrentLifesCount;
        }
        return SecondThrower.ObjectThrower.CurrentLifesCount;
    } 
    
    public bool CheckCurrentPlayerUseShield() {
        if (IsFirstThrowerStep) {
            return FirstThrower.ObjectThrower.IsShielded;
        }
        return SecondThrower.ObjectThrower.IsShielded;
    } 
    
    

    
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
        if(!MainPlayerPlay) return;
        Debug.Log("Focus camera");
        _camera.SetCameraToPlayThrow(obj);
    }

    public void InitForNewGame(bool firstPlayerBot, bool secondPlayerBot) {
        Debug.Log("firstPlayerBot " + firstPlayerBot);
        Debug.Log("secondPlayerBot " + secondPlayerBot);

        AllowToPlay = false;
        MainPlayerPlay = !firstPlayerBot;

        if (!firstPlayerBot) {
            FirstThrower = _mainPlayer;
            GetReadyPlayer(_mainPlayer, true);
        }
        else {
            FirstThrower = _botsMainManager.GetRandomBotToBattle(false);
            GetReadyPlayer(FirstThrower, true);
            // Это битва 2х ботов
            // - сделать аим у ботов 100%
        }

        // Второй игрок всегда ботяра, ток с настроенным поведением
        SecondThrower = _botsMainManager.GetRandomBotToBattle(!secondPlayerBot);
        GetReadyPlayer(SecondThrower, false);
        
        
        FirstThrower.ObjectThrower.InitToNewGame(true);
        FirstThrower.ObjectThrower.SetBotBehaviour(_bot1, SecondThrower.ObjectThrower, firstPlayerBot);
        
        SecondThrower.ObjectThrower.InitToNewGame(false);
        SecondThrower.ObjectThrower.SetBotBehaviour(_bot2, FirstThrower.ObjectThrower, secondPlayerBot);
        SecondThrower.ObjectThrower.SetBotBehaviour(_bot2, FirstThrower.ObjectThrower, secondPlayerBot);
        
        
        GoBattle().Forget();
    }

    public void SetGameOver() {
        Debug.Log("SetGameOver");
        if (FirstThrower != null) FirstThrower.ObjectThrower.SetDead();
        if (SecondThrower != null) SecondThrower.ObjectThrower.SetDead();
        _bot1.DisposeBot();
        _bot2.DisposeBot();
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
        if (MainPlayerPlay) {
            FocusCamera(_leftCameraFocus);
            _startBattleView.StartBattleAnimation(); 
            await UniTask.WaitWhile(() => _startBattleView.AnimationPlayNow);            
        }        
        
        
        
        FirstThrower.ObjectThrower.SetAllowToThrow(false);
        SecondThrower.ObjectThrower.SetAllowToThrow(false);
        while(!GameIsOver) {
            await PlayerStepAsync(FirstThrower.ObjectThrower, _leftCameraFocus, true);
            await PlayerStepAsync(SecondThrower.ObjectThrower, _rightCameraFocus, false);
        }

        if (MainPlayerPlay) {
            _gameOverShower.ShowResults();
            await UniTask.WaitWhile(() => _gameOverShower.ResultWindowShowing);
        }

        SetSpawnState();
        _throwGameStarter.GameOver();
        Debug.Log("Игра закончилась !");
    }

    private async UniTask PlayerStepAsync(ObjectThrower thrower, Transform pointToCameraFocus, bool isFirstThrowerStep) {
        if(GameIsOver) return;

        Debug.Log("NewPlayerTurn, BotTurnNow = " + !thrower.PlayerHandle);
        BotTurnNow = !thrower.PlayerHandle;
        // Анимация шага игрока
        FocusCamera(pointToCameraFocus);
        if (MainPlayerPlay) {
            _playersStepView.ShowPlayerStep(isFirstThrowerStep);
            await UniTask.WaitWhile(() => _playersStepView.AnimationIsShowing);
            await WaitThrowableObjectGet();
        }
        else {
            _newThrowableObjectInRoulette = _throwObjectsIniter.GetRandomToyForBot;
        }
        
        AllowToPlay = true;
        IsFirstThrowerStep = isFirstThrowerStep;
        NewPlayerTurn?.Invoke();
        _stepIsOver = false;
        thrower.SetAllowToThrow(true);
        
        _timerToThrowStep.StartTimer(_secondsInStep);
        _windChooser.UpdateWind();
        
        
        thrower.Damageable.SetInvinsible(true);
        await UniTask.WaitUntil(() => _stepIsOver || GameIsOver);
        AllowToPlay = false;
        thrower.Damageable.SetInvinsible(false);
        
        
        thrower.SetAllowToThrow(false);
        if (!thrower.PlayerHandle) {
            BotTurnNow = false;
        }
    }

    private async UniTask WaitThrowableObjectGet() {
        _roulette.gameObject.ActiveSelf();
        await UniTask.NextFrame();
        _roulette.ResetItemPosition();
        _roulette.FillRandomSkins();
        _newThrowableObjectInRoulette = await _roulette.SpinAsync();
        _roulette.gameObject.DisactiveSelf();
    }


    private void SetSpawnState() {
        Debug.Log("SetSpawnState");
        if (FirstThrower != null && FirstThrower.IsPlaying) {
            FirstThrower.SetPlayStatus(false);
            FirstThrower.ObjectThrower.SetAllowToThrow(false);
            FirstThrower.ObjectThrower.SetUnshielded();
        }

        if (SecondThrower != null && SecondThrower.IsPlaying) {
            SecondThrower.SetPlayStatus(false);
            SecondThrower.ObjectThrower.SetAllowToThrow(false);
            SecondThrower.ObjectThrower.SetUnshielded();
        }
        
        if (MainPlayerPlay) {
            _camera.ResetCameraBeforePlay();
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
        
        _throwerCalculator.ThrowNewObject(
            thrower.AngleToThrow, 
            _newThrowableObjectInRoulette, 
            thrower.ThrowPoint, enemyPoint
        );
        
        _stepIsOver = false;
    }
    
    
}