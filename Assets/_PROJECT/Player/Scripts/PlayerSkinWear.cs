using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerSkinWear : MonoBehaviour { 
    [SerializeField] private Transform _playerWearSkinParent;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _currentSkin;
    [SerializeField] private PlayerAnimator _playerAnimator;
    
    private Coroutine _changeSkinRoutine;

    
    [Inject] private IInputActivity _inputActivity;
    [Inject] private List<SkinItemConfig> _skins; 
    [Inject] private PlayerSkinInventory _playerSkinInventory;

    private void OnEnable() {
        _playerSkinInventory.SkinEquipped += WearNewSkinVisual;
    }

    private SkinItemConfig GetSkinItemById(string id) 
        => _skins.Find(s => s.Id == id);


    private void Start() {
        WearNewSkinVisual(GetSkinItemById(_playerSkinInventory.CurrentSkinId));
    }

    private void WearNewSkinVisual(SkinItemConfig playerSkin) {
    
        // Создаем временный скин для мгновенного отображения
        if (_currentSkin != null) {
            Destroy(_currentSkin);
        }
    
        
        var tempSkin = Instantiate(playerSkin.SkinPrefab, _playerWearSkinParent);
        _currentSkin = tempSkin;

        var tempController = tempSkin.GetComponent<SkinElementsController>();
        _playerAnimator.SetSkinElementsController(tempController);
    
        // Запускаем корутину для финальной замены с аватаром
        if (_changeSkinRoutine != null) {
            StopCoroutine(_changeSkinRoutine);
        }

        _changeSkinRoutine = StartCoroutine(ChangeSkinRoutine(playerSkin));
    }

    private IEnumerator ChangeSkinRoutine(SkinItemConfig skin) {
        _inputActivity.Disable();

        // Не уничтожаем tempSkin здесь, так как это и есть текущий скин
        _animator.avatar = null;
    
        yield return null;
    
        // Обновляем аватар
        _animator.avatar = skin.Avatar;
        _inputActivity.Enable();
    }
}
