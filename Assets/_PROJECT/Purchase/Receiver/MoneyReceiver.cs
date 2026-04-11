using LuringPlayer_M;
using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class MoneyReceiver : AwardReceiver {
    [SerializeField] protected int _newMoney;
    [Inject] protected PlayerBank _bank;
    [Inject] protected IGameSave _saver;

    
    public override void Receive()
    {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _bank != null);
        _bank.AddMoney(_newMoney);
        _saver.Save();
    }
}