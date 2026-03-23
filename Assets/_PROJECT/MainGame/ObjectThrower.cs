using UnityEngine;
using Zenject;


/// <summary>
/// Делает бросок с помощью ObjectThrowerBase
/// </summary>
public class ObjectThrower : MonoBehaviour {
    [SerializeField] private TrajectoryVisualize3D thrower;
    [field: Header("Игрок или бот управляет")]
    [field: SerializeField] public bool PlayerHandle;

    // Расчет физики полета и тп
    [Inject] private ObjectThrowerCalculator _objectThrowerBase;

    public void DoThrow() {
        if (PlayerHandle) {
            
        }
        else {
            
        }
    }
}
