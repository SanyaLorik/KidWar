using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CanvasOpenerWithTrigger : TriggerBehaviourBase {
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    [Inject] AdvTimerStarter _advTimerStarter;
    
    private void OnEnable() {
        if (_openButton != null) {
            _openButton.onClick.AddListener(_canvas.ActiveSelf);
        }

        if (_closeButton != null) {
            _closeButton.onClick.AddListener(_canvas.DisactiveSelf);
        }
    }

    protected override void PlayerBehaviourOnEnter() {
        _advTimerStarter.DisableTimer();
        _delayedTrigger.DelayedTriggerAction(TriggerAction); 
    }
    
    protected override void PlayerBehaviourOnExit() {
        _advTimerStarter.EnableTimer();
        _delayedTrigger.CancelTriggerAction();
    }

    private void TriggerAction() {
        _canvas.ActiveSelf();
        GameEvents.TriggerUseInvoke();
    }
}