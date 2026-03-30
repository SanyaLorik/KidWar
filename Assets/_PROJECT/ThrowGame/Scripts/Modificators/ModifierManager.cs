using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ModifierManager : MonoBehaviour {
    [SerializeField] private List<ModifierChanger> _leftModifierChanger;
    [SerializeField] private List<ModifierChanger> _rightModifierChanger;
    [Range(0,1), SerializeField] private float _chanseToBotUseModifier;

    private ModifierChanger _choosedModifierChanger;
    private readonly ThrowableModifierDefault _defaultModifier = new();
    
    
    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BattleManager _battleManager;

        
    public IThrowableModifier CurrentModifier { get; private set; }

    private void OnEnable() {
        _gameStarter.GameStarted += GameStarted;
        _battleManager.NewPlayerTurn += NewStep;
        _calculator.ObjectThrowed += OnObjectThrowed;
    }

    private void OnObjectThrowed(Transform _) {
        if (CurrentModifier != _defaultModifier) {
            _choosedModifierChanger.SetUnvailable(true);
        }
    }

    public void TrySetModifier(IThrowableModifier modifier, ModifierChanger modifierChanger) {
        if(_battleManager.MainPlayerPlay && _battleManager.BotTurnNow) return;
        // Проверка на соответствие игрока и запроса
        if (IsLeftPlayerModifier(modifierChanger)) {
            if (_battleManager.IsFirstThrowerStep == true) {
                Debug.Log("Установка или снятие модификатора для левого игрока");
                SetModifierAfterCheck(modifier, modifierChanger);
            }
        }
        else {
            if (_battleManager.IsFirstThrowerStep == false) {
                Debug.Log("Установка или снятие модификатора для правого игрока");
                SetModifierAfterCheck(modifier, modifierChanger);
            }
        }
    }

    public void UseModifierForBot() {
        if (Random.value > _chanseToBotUseModifier) return;
        List<ModifierChanger> modifierChangersList = _battleManager.IsFirstThrowerStep ? 
            _leftModifierChanger 
            : 
            _rightModifierChanger;
        
        int startIndex =  Random.Range(0, modifierChangersList.Count-1);
        for (int i = 0; i < modifierChangersList.Count; i++) {
            int index = (startIndex + i) % modifierChangersList.Count;
            ModifierChanger modifierChanger = modifierChangersList[index];
            
            if (modifierChanger.IsAvailable) {
                SetModifierAfterCheck(modifierChanger.Modifier, modifierChanger);
                Debug.Log("Выбран модификатор");
                return;
            }
        }
        Debug.Log("Бот не нашел модификатор");
    }

    private void SetModifierAfterCheck(IThrowableModifier modifier, ModifierChanger modifierChanger) {
        // Повторное нажатие
        if (CurrentModifier == modifier) {
            SetModifier(_defaultModifier);
            modifierChanger.HidePointer();  
        }
        else {
            SetModifier(modifier);
            modifierChanger.ShowPointer();  
        }
    }

    private bool IsLeftPlayerModifier(ModifierChanger modifierChanger) {
        return _leftModifierChanger.FindIndex(mc => mc == modifierChanger) != -1;
    }

    private void SetModifier(IThrowableModifier modifier) {
        // Hide old
        if (_choosedModifierChanger != null) {
            _choosedModifierChanger.HidePointer();
        }
        // Left
        _choosedModifierChanger = _battleManager.IsFirstThrowerStep ? 
            _leftModifierChanger.Find(mc => mc.Modifier.GetType() == modifier.GetType()) 
            :
            _rightModifierChanger.Find(mc => mc.Modifier.GetType() == modifier.GetType());
        CurrentModifier = modifier;
    }

    private void GameStarted(bool gameStarted) {
        if (gameStarted) {
            _leftModifierChanger.ForEach(m => m.SetAvailable());
            _leftModifierChanger.ForEach(m => m.HidePointer());
            
            _rightModifierChanger.ForEach(m => m.SetAvailable());
            _rightModifierChanger.ForEach(m => m.HidePointer());
            NewStep();
        }
    }

    private void NewStep() {
        Debug.Log("Установка дефолт модификатора");
        CurrentModifier = _defaultModifier;
        if (_choosedModifierChanger != null) {
            _choosedModifierChanger.HidePointer();
        }
        _choosedModifierChanger = null;
    }
}
