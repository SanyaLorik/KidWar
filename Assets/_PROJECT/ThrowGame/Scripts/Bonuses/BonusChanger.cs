using System;
using TMPro;
using UnityEngine;
using Zenject;

public class BonusChanger : UsableItemBase {
    [SerializeReference, SubclassSelector] private IBonus _bonus;
    [SerializeField] private int _bonusCounts;
    [SerializeField] private TextMeshProUGUI _countText;
    
    public IBonus Bonus => _bonus;
    public int BonusCount => _bonusCounts;
    
    [Inject] private BonusManager _bonusManager;
    [Inject] private DiContainer _diContainer;

    
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(_bonus);
    }
    
    
    private void Start() {
        ChangeVisualCount();
    }


    public override void TryUse() {
        if (!IsAvailable) {
            Debug.Log("Бонус на перезарядке именно что");
            return;
        }

        if (_bonusCounts == 0) {
            Debug.Log("Бонусов нема");
            return;
        }
        _bonusManager.UseBonusByClick(_bonus, this);
    }

    public void CheckAvailable() {
        if (_bonusCounts == 0) {
            SetUnvailable();
        }
        else {
            SetAvailable();
        }
    }

    
    public void AddBonusCount(int newBonusCount) {
        _bonusCounts += newBonusCount;
        ChangeVisualCount();
    }

    
    public void GetOneBonus() {
        if (_bonusCounts != 0) {
            _bonusCounts--;
        }
        ChangeVisualCount();
    }

    
    private void ChangeVisualCount() {
        _countText.text = _bonusCounts.ToString();
        CheckAvailable();
    }
    
}