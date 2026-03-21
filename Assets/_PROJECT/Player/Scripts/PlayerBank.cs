using System;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    public event Action<long> BankNewMoneyPlus;
    public event Action<long> BankNewMoneyMinus;

    // [Inject] IGameSave<GameSavePC> _gameSave;

    // public long PlayerCapital {
    //     get => _gameSave.GetSave.Money;
    //     private set => _gameSave.GetSave.Money = value;
    // }
    
    public long PlayerCapital;

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
