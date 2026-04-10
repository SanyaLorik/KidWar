using System;
using TMPro;
using UnityEngine;
using Zenject;

public class ParkourCompleteTrigger : TriggerBehaviourBase {
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private int _parkourReward;
    
    public event Action ParkourCompleted;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] NumberFormatter _formatter;
    [Inject] PlayerBank _bank;

    private void Start() {
        SetParkourRewardText();
    }

    private void SetParkourRewardText() {
        _rewardText.text = _formatter.ValuteFormatter(_parkourReward);
    }

    protected override void PlayerBehaviourOnEnter() {
        _delayedTrigger.DelayedTriggerAction(TriggerAction); 
    }
    
    protected override void PlayerBehaviourOnExit() {
        _delayedTrigger.CancelTriggerAction();
    }

    private void TriggerAction() {
        _bank.AddMoney(_parkourReward);
        _playerMovement.TeleportInSpawn();
        ParkourCompleted?.Invoke();
    }
}