using LuringPlayer_M;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class SkinReceiver : AwardReceiver
{
    [SerializeField] private SkinItemConfig _skinInStore;
    
    [Inject] private PlayerSkinInventory _playerSkinInventory;
    [Inject] private List<SkinItemConfig> _skinConfigs;
    
    public override void Receive()
    {
        BindReceiveAsync();
    }
    
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _playerSkinInventory != null && _skinConfigs != null);
        if (_playerSkinInventory.SkinIsBought(_skinInStore.Id)) 
        {
            Debug.Log("Куплен уже имеющийся скин " + _skinInStore.Id);
        }
        else 
        {
            _playerSkinInventory.UnlockSkin(_skinInStore);
        }
        _playerSkinInventory.EquipSkin(_skinInStore);
    }
}
