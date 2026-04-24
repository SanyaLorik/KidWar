using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CanvasOpenerWithTrigger : TriggerBehaviourBase {
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    [Inject] AdvHelper _advHelper;
    
    private void OnEnable() {
        if (_openButton != null) {
            _openButton.onClick.AddListener(_canvas.ActiveSelf);
        }

        if (_closeButton != null) {
            _closeButton.onClick.AddListener(_canvas.DisactiveSelf);
        }
    }

    protected override void PlayerBehaviourOnEnter() {
        _advHelper.DisableTimer();
        _delayedTrigger.DelayedTriggerAction(TriggerAction); 
    }
    
    protected override void PlayerBehaviourOnExit() {
        _advHelper.EnableTimer();
        _delayedTrigger.CancelTriggerAction();
    }

    private void TriggerAction() {
        _canvas.ActiveSelf();
        GameEvents.TriggerUseInvoke();
    }
}