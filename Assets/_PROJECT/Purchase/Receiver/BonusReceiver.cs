using LuringPlayer_M;
using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class BonusReceiver : AwardReceiver
{
    [SerializeField] private BonusItemSO _bonusItem;
    [SerializeField] private int _count;
    
    [Inject] IGameSave _saver;
    
    public override void Receive()
    {
        BindReceiveAsync();
    }
    
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _saver != null);
        _saver.GetSave<GameSave>().AddNewBonusCounts(_bonusItem.Id, _count);
        _saver.Save();
    }
}