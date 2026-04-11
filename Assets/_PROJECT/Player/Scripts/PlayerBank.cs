using System;
using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI[] _moneyTexts;
    [SerializeField] private int _moneyStartCapital;
    public event Action<long> BankNewMoneyPlus;
    public event Action<long> BankNewMoneyMinus;

    private GameSave Saves => _gameSave.GetSave<GameSave>();
    
    [Inject] IGameSave _gameSave;
    [Inject] NumberFormatter _formatter;

    
    public long PlayerCapital {
        get => Saves.Money;
        private set {
            Saves.Money = value;
            _moneyTexts.ForEach(t => t.text = _formatter.ValuteFormatter(PlayerCapital));
            _gameSave.Save();
        }
    }
    

    private void Start() {
        _moneyTexts.ForEach(t => t.text = _formatter.ValuteFormatter(PlayerCapital));
        if (!Saves.TutorialPassed) {
            PlayerCapital += _moneyStartCapital;
        }
    }

    
    public void AddMoney(long amount) {
        if (amount <= 0) {
            Debug.LogError("Попытка добавить <= 0");
            return;
        }

        long newValue;

        // защита от переполнения
        if (PlayerCapital > long.MaxValue - amount) {
            newValue = long.MaxValue;
        } 
        else {
            newValue = PlayerCapital + amount;
        }

        PlayerCapital = newValue;
        BankNewMoneyPlus?.Invoke(amount);
    }

    public void SpendMoney(long amount) {
        if (amount <= 0) {
            Debug.LogError("Попытка потратить <= 0");
            return;
        }

        if (PlayerCapital < amount) {
            Debug.LogError("Недостаточно денег");
            PlayerCapital = 0;
        } 
        else {
            PlayerCapital -= amount;
        }

        BankNewMoneyMinus?.Invoke(amount);
    }

    public bool CanBuy(long amount) =>
        PlayerCapital >= amount;

}
