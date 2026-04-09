using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ShopSkinsIniter : MonoBehaviour  {
    [SerializeField] private List<ShopSkinItem> _shopSkinItems;

    private bool _injected = false; 
        
        
    [Inject] private DiContainer _diContainer;
    
    [Inject]
    private void Init() {
        InjectItems();
        WaitWhileNotInjectAsync(InitializeAllItems).Forget();
    }
    
    private void OnEnable() {
        if (_injected) {
            _shopSkinItems.ForEach(i => i.CheckStatus());
        }
        else {
            WaitWhileNotInjectAsync(CheckAllStatuses).Forget();
        }
    }


    private void InjectItems() {
        foreach (var shopSkinItem in _shopSkinItems) {
            _diContainer.QueueForInject(shopSkinItem);
        }
        _injected = true;
    }

    
    private async UniTask WaitWhileNotInjectAsync(Action action) {
        await UniTask.WaitUntil(() => _injected);
        action?.Invoke();
    }
    
    
    private void InitializeAllItems() {
        _shopSkinItems.ForEach(i => i.Initialize(this));
    }

    
    public void CheckAllStatuses() {
        _shopSkinItems.ForEach(i => i.CheckStatus());
    }
}