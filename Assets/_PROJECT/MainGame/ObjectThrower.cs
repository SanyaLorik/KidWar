using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


/// <summary>
/// Делает бросок с помощью ObjectThrowerBase
/// </summary>
public class ObjectThrower : MonoBehaviour {
    [SerializeField] private TrajectoryVisualize3D _throwVisualize;
    [field: Header("Игрок или бот управляет")]
    [field: SerializeField] public bool PlayerHandle;


    private Mouse _mouse;
    public bool AllowToThrow { get; private set; }

    public Transform ThrowPoint =>  _throwVisualize.ThrowPoint;
    public Transform PointToCameraFocus =>  _throwVisualize.PointToCameraFocus;
    public float AngleToThrow =>  _throwVisualize.CurrentVerticalAngle;

    // Расчет физики полета и тп
    [Inject] private BattleManager _battleManager;

    
    
    private void Start() {
        _mouse = Mouse.current;
    }

    
    private void Update() {
        if (_mouse.leftButton.wasReleasedThisFrame && AllowToThrow) {
            _battleManager.DoStep(this);
        }
    }

    
    public void SetAllowToThrow(bool state) {
        AllowToThrow = state;
        _throwVisualize.SetActiveTrajectoryVisual(state);
    }


}
