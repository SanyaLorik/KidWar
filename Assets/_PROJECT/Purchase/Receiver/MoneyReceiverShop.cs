using System;
using Cysharp.Threading.Tasks;

[Serializable]
public class MoneyReceiverShop : MoneyReceiver {
    public override void Receive() 
    {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _bank != null);
        _bank.AddMoney(_newMoney);
        _saver.GetSave<GameSave>().IsBoughtPurchase = true;
        _saver.Save();
    }
}