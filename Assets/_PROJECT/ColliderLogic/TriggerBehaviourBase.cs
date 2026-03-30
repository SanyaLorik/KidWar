using UnityEngine;

public abstract class TriggerBehaviourBase : MonoBehaviour, ITriggerBehaviour {
    public void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement playerMovement)) return;
        OnPlayerEnter();
    }

    public void OnTriggerExit(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement playerMovement)) return;
        OnPlayerExit();
    }

    public void OnPlayerEnter() {
        PlayerBehaviourOnEnter();
    }

    public void OnPlayerExit() {
        PlayerBehaviourOnExit();
    }

    protected abstract void PlayerBehaviourOnEnter();
    protected abstract void PlayerBehaviourOnExit();
}