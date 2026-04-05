using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOpenerWithTrigger : TriggerBehaviourBase {
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    private void OnEnable() {
        _openButton.onClick.AddListener(_canvas.ActiveSelf);
        _closeButton.onClick.AddListener(_canvas.DisactiveSelf);
    }

    protected override void PlayerBehaviourOnEnter() {
        _delayedTrigger.DelayedTriggerAction(TriggerAction); 
    }
    
    protected override void PlayerBehaviourOnExit() {
        _delayedTrigger.CancelTriggerAction();
    }

    private void TriggerAction() {
        _canvas.ActiveSelf();
    }
}