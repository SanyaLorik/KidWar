using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public enum TutorialStep {
    Aiming,
    ChargePower,
    ThrowAndCheckHit,
    UseHeal,
    ActivateExplosionMode,
}


public class TutorialManager : MonoBehaviour {
    [SerializeField] private Narrator _narrator;
    [Header("Timings")]
    [SerializeField] private float _timeToAiming;
    [SerializeField] private float _timeToCharge;
    [SerializeField] private float _timeToRouletteChoose;
    [SerializeField] private int _startBonusesCount;
    
    
    private bool _playerUseHeal;
    private bool _playerChooseExplosionModifier;

    
    public event Action NewTutorialStep; 
    public bool TutorialPassed => Saves.TutorialPassed;
    public bool AimOn { get; private set; }
    
    private GameSave Saves => _saver.GetSave<GameSave>();

    [Inject] private IGameSave _saver; 
    [Inject] private ThrowGameStarter _gameStarter; 
    [Inject] private BattleManager _battleManager; 
    [Inject] private ThrowGameStarter _throwGameStarter; 
    [Inject] private InputThrowGame _inputThrowGame;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BonusManager _bonusManager;
    [Inject] private ModifierManager _modifierManager;
    [Inject] private PlayersStepView _stepView;


    public void OnEnable() {
        if (!TutorialPassed) {
            _battleManager.GameStarted += StartTutorial;
            _bonusManager.BonusUsed += OnBonusUsed;
            _modifierManager.ModifierChoosed += OnChoosedModifier;
            _throwGameStarter.GameStarted += OnGameStarted;
        }
    }
    

    private void Start() {
        _narrator.Disactive();
        if (!TutorialPassed) {
            _bonusManager.InitBonusesCount(_startBonusesCount);
            _bonusManager.SetBonusesEnable(false);
            _modifierManager.SetModifiersEnable(false);
        }
    }
    

    private void OnGameStarted(bool started) {
        if (!started) {
            OnTutorialEnd();
        }
    }

    private void OnChoosedModifier(IThrowableModifier modifier) {
        Debug.Log($"modifier = {modifier} modifier is ThrowableModifierExplosion = {modifier is ThrowableModifierExplosion}");
        if (modifier is ThrowableModifierExplosion) {
            _playerChooseExplosionModifier = true;
        } 
    }

    
    private void OnBonusUsed(IBonus bonus) {
        if (bonus is HealBonus) {
            _playerUseHeal = true;
        } 
    }


    private void StartTutorial() {
        TutorialStartAsync().Forget();
    }

    
    private async UniTask TutorialStartAsync() {
        // Зажми палец в этой зоне и двигай им — наведись на врага
        NewTutorialStep?.Invoke();
        await AimingStep();
        
        // Шкала силы = дальность броска. Чем больше зарядишь — тем дальше летит
        NewTutorialStep?.Invoke();
        await ChargePowerStep();
        
        // Отпусти палец — и снаряд полетит прямо в цель
        NewTutorialStep?.Invoke();
        await ThrowAndCheckHitStep();
        
        // Пополни свое здоровье, нажми на аптечку 
        NewTutorialStep?.Invoke();
        await HealStep();
        
        // Нажми на «Взрыв» перед броском — урон станет мощнее
        NewTutorialStep?.Invoke();
        await ExplosionStep();
    }

    
    private async UniTask AimingStep() {
        await UniTask.WaitForSeconds(_timeToRouletteChoose);
        _narrator.Active();
        _narrator.SetTutorialText(TutorialStep.Aiming);
        _narrator.ShowScreenFinger();
        await UniTask.WaitWhile(() => !_inputThrowGame.Downed);
        await UniTask.WaitForSeconds(_timeToAiming);
    }    
    
    
    private async UniTask ChargePowerStep() {
        _narrator.SetTutorialText(TutorialStep.ChargePower);
        await UniTask.WaitForSeconds(_timeToCharge);
    }
    
    
    private async UniTask ThrowAndCheckHitStep() {
        _narrator.SetTutorialText(TutorialStep.ThrowAndCheckHit);
        await UniTask.WaitWhile(() => _battleManager.PlayerStepInPvb);
        _narrator.Disactive();
        _inputThrowGame.SetAllowToThrowWhileTutorial(false);
    }
    
    
    private async UniTask HealStep() {
        // Ход врага, ждем...
        await UniTask.WaitWhile(() => !_battleManager.PlayerStepInPvb);
        
        await UniTask.WaitForSeconds(_timeToRouletteChoose + _stepView.TimeToShowBig + _stepView.TimeToHideBig);
        
        _bonusManager.SetBonusesEnable(true);
        _bonusManager.SetPlayerEnableOnlyHeal();
        
        _narrator.Active();
        _narrator.ShowHealFinger();
        _narrator.SetTutorialText(TutorialStep.UseHeal);
        
        // Ждем юза хилки именно что
        await UniTask.WaitWhile(() => !_playerUseHeal);
        _narrator.CheckToDestroyCurrentFinger();

    }
    
    
    private async UniTask ExplosionStep() {
        // Ждем пока юзнет модифиактор
        _modifierManager.SetModifiersEnable(true);
        _modifierManager.SetPlayerEnableOnlyExplosion();
        
        _narrator.ShowExplosionFinger();
        _narrator.SetTutorialText(TutorialStep.ActivateExplosionMode);
        await UniTask.WaitWhile(() => !_playerChooseExplosionModifier);
        _narrator.CheckToDestroyCurrentFinger();
        AimOn = true;
        Debug.Log("Аим включен");
        _inputThrowGame.SetAllowToThrowWhileTutorial(true);
        _narrator.ShowScreenFinger();
        await UniTask.WaitWhile(() => !_calculator.ObjectInFly);
        _narrator.Disactive();
    }

    
    private void OnTutorialEnd() {
        Debug.Log("OnTutorialEnd");
        AimOn = false;
        Saves.TutorialPassed = true;
        _saver.Save();
        _narrator.DisableNarrator();
        _battleManager.GameStarted -= StartTutorial;
        _bonusManager.BonusUsed -= OnBonusUsed;
        _modifierManager.ModifierChoosed -= OnChoosedModifier;
        _throwGameStarter.GameStarted -= OnGameStarted;
    }

}
