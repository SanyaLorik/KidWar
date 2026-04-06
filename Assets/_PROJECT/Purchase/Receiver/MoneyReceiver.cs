using LuringPlayer_M;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class MoneyReceiver : AwardReceiver {
    [SerializeField] private int _newMoney;
    [Inject] private PlayerBank _bank;
    
    public override void Receive()
    {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _bank != null);
        _bank.AddMoney(_newMoney);
    }
}
