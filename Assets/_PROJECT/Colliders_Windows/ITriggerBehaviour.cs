using UnityEngine;

public interface ITriggerBehaviour {
    public void OnPlayerEnter();
    public void OnPlayerExit();
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerExit(Collider collider);
}