using System;
using TMPro;
using UnityEngine;
using Zenject;

public class ParkourCompleteTrigger : TriggerBehaviourBase {
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private DelayedTrigger _delayedTrigger;
    
    public event Action ParkourCompleted;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] NumberFormatter _formatter;

    public void SetParkourRewardText(int rewardText) {
        _rewardText.text = _formatter.ValuteFormatter(rewardText);
    }

    protected override void PlayerBehaviourOnEnter() {
        _delayedTrigger.DelayedTriggerAction(TriggerAction); 
    }
    
    protected override void PlayerBehaviourOnExit() {
        _delayedTrigger.CancelTriggerAction();
    }

    private void TriggerAction() {
        _playerMovement.TeleportInSpawn();
        ParkourCompleted?.Invoke();
    }
}
