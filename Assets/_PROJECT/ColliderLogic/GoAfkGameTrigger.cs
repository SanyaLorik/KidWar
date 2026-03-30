using UnityEngine;
using Zenject;

public class GoDuoGameTrigger : TriggerBehaviourBase {
    [SerializeField] private bool _afkStatusOnEnter;
    
    [Inject] ThrowGameStarter _gameStarter;
    
    
    protected override void PlayerBehaviourOnEnter() {
        _gameStarter.ChangeAfkStatus(_afkStatusOnEnter);
    }

    protected override void PlayerBehaviourOnExit() { }
}