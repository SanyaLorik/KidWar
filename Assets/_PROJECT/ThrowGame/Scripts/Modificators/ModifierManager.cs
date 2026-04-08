using System;
using System.Collections.Generic;
using System.Linq;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ModifierManager : MonoBehaviour {
    [SerializeField] private List<ModifierChanger> _leftModifierChanger;
    [SerializeField] private List<ModifierChanger> _rightModifierChanger;
    [SerializeField] private GameObject[] _allModifiersObjects;
    [Range(0,1), SerializeField] private float _chanseToTryAgainFindModifierBot;
    [SerializeField] private List<ItemValueBase<IThrowableModifier>> _modifierValues;
    
    private ModifierChanger _choosedModifierChanger;
    private readonly ThrowableModifierDefault _defaultModifier = new();
    
    private float _totalWeight;
    private bool _showModifiers = true;
    public IThrowableModifier CurrentModifier { get; private set; }
    public event Action<IThrowableModifier> ModifierChoosed;


    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private BattleManager _battleManager;
    [Inject] private TutorialManager _tutorialManager;

        

    private void OnEnable() {
        _battleManager.NewPlayerTurn += OnNewPlayerTurn;
        _gameStarter.GameStarted += GameStarted;
        _battleManager.NewPlayerTurn += NewStep;
        _calculator.ObjectThrowed += OnObjectThrowed;
    }

    private void Start() {
        CalculateValueDivider();
    }
    
    public void SetModifiersEnable(bool state) {
        _allModifiersObjects.ForEach(m => m.gameObject.SetActive(state));
        _showModifiers = state;
    } 
    
    public void SetPlayerEnableOnlyExplosion() {
        _leftModifierChanger.ForEach(b => b.SetUnvailable());
        _rightModifierChanger.ForEach(b => b.SetUnvailable());
        foreach (var leftModifier in _leftModifierChanger) {
            if (leftModifier.Modifier is ThrowableModifierExplosion) {
                leftModifier.SetAvailable();
            }
            else {
                leftModifier.SetUnvailable();
            }
        }
    }
    
    
    private void OnNewPlayerTurn() {
        if(!_battleManager.MainPlayerPlay) return;
        if(!_showModifiers) return;
        
        _leftModifierChanger.ForEach(b => b.SetVisualGray(!_battleManager.IsFirstThrowerStep));
        _rightModifierChanger.ForEach(b => b.SetVisualGray(_battleManager.IsFirstThrowerStep));
        
    }

    private void CalculateValueDivider() {
        _totalWeight = _modifierValues.Sum(m => m.Weight);
    }
    
    private void OnObjectThrowed(Transform _) {
        if (CurrentModifier != _defaultModifier) {
            _choosedModifierChanger.SetUnvailable(true);
        }
    }

    public void TrySetModifier(IThrowableModifier modifier, ModifierChanger modifierChanger) {
        if(_battleManager.MainPlayerPlay && _battleManager.BotTurnNow) return;
        if(!_battleManager.AllowToPlay) return;
        // Проверка на соответствие игрока и запроса
        if (IsLeftPlayerModifier(modifierChanger)) {
            if (_battleManager.IsFirstThrowerStep == true) {
                Debug.Log("Установка или снятие модификатора для левого игрока");
                SetModifierAfterCheck(modifier, modifierChanger);
                ModifierChoosed?.Invoke(modifier);
                // Даем игроку выбрать модифиактор и офаем его, чтоб он не мог отжать
                if (!_tutorialManager.TutorialPassed) {
                    modifierChanger.SetUnvailable();
                }
            }
        }
        else {
            if (_battleManager.IsFirstThrowerStep == false) {
                Debug.Log("Установка или снятие модификатора для правого игрока");
                SetModifierAfterCheck(modifier, modifierChanger);
                ModifierChoosed?.Invoke(modifier);
            }
        }
    }

    public void UseModifierForBot() {
        if(!_tutorialManager.TutorialPassed) return;
        List<ModifierChanger> modifierChangersList = _battleManager.IsFirstThrowerStep ? 
            _leftModifierChanger 
            : 
            _rightModifierChanger;
        // Фаза 1
        if (TryUseRandomModifierForBot(modifierChangersList)) return;
        // Фаза 2, модифиактор на перезарядке
        Debug.Log("Модификатор на перезарядке, бот выбирает другой");
        if (TryUseRandomModifierForBot(modifierChangersList))  return;
        Debug.Log("Бот не выбрал модифиактор");
    }


    public void ResetAllPlayerModifiers() {
        List<ModifierChanger> modifierChangersList = _battleManager.IsFirstThrowerStep ? 
            _leftModifierChanger 
            : 
            _rightModifierChanger;
        
        modifierChangersList.ForEach(m => m.SetAvailable());
    }

    private bool TryUseRandomModifierForBot(List<ModifierChanger> modifierChangersList) {
        IThrowableModifier modifier = ItemValueBase.GetRandomItemByWeight(_modifierValues, _totalWeight);
        ModifierChanger modifierChanger = modifierChangersList.Find(m => m.Modifier.GetType() == modifier.GetType());
        if (modifierChanger.IsAvailable) {
            SetModifierAfterCheck(modifierChanger.Modifier, modifierChanger);
            // Debug.Log("Выбран модификатор: " + modifierChanger.Modifier.GetType());
            return true;
        }
        // Debug.Log("НЕ Выбран модификатор: is not available" + modifierChanger.Modifier.GetType());
        return false;
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

            if (!_battleManager.MainPlayerPlay) {
                //
            }
            
        }
    }

    private void NewStep() {
        // Debug.Log("Установка дефолт модификатора");
        CurrentModifier = _defaultModifier;
        if (_choosedModifierChanger != null) {
            _choosedModifierChanger.HidePointer();
        }
        _choosedModifierChanger = null;
    }
}
