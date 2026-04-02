using SanyaBeerExtension;
using UnityEngine;

public class ThrowObjectsIniter : MonoBehaviour {
    [SerializeField] private ThrowableObject[] _throwableObjects;

    public ThrowableObject GetRandomToyForBot => _throwableObjects.GetRandomElement();
}
