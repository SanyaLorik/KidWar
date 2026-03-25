using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


/// <summary>
/// Делает бросок с помощью ObjectThrowerBase
/// </summary>
public class ObjectThrower : MonoBehaviour {
    [SerializeField] private TrajectoryVisualize3D _throwVisualize;

    [field: Header("Игрок или бот управляет")]
    [field: SerializeField] public bool PlayerHandle { get; set; }
    [SerializeField] private bool _stayInLeft;

    private Mouse _mouse;
    private bool _allowToThrow;

    public int CurrentLifesCount { get; private set; }

    public Transform ThrowPoint =>  _throwVisualize.ThrowPoint;
    public Transform PointToCameraFocus =>  _throwVisualize.PointToCameraFocus;
    public float AngleToThrow =>  _throwVisualize.CurrentVerticalAngle;

    // Расчет физики полета и тп
    [Inject] private BattleManager _battleManager;
    [Inject] private HpView _hpView;

    
    private void Start() {
        _mouse = Mouse.current;
    }

    
    private void Update() {
        if (_mouse.leftButton.wasReleasedThisFrame && _allowToThrow) {
            _battleManager.DoStep(this);
        }
    }

    public void SetStartHp(int hp) {
        CurrentLifesCount = hp;
        _hpView.ChangeHp(CurrentLifesCount, _stayInLeft);
    }
    
    public void MinusHp(int hp) {
        if (_allowToThrow) {
            Debug.Log("Сам по себе попал, ну можно и отключить...");
            return;
        }
        CurrentLifesCount -= hp;
        CurrentLifesCount = Mathf.Max(0, CurrentLifesCount);
        Debug.Log($"Попал! Минус {hp} хп, щас HP = {CurrentLifesCount} ");
        _hpView.ChangeHp(CurrentLifesCount, _stayInLeft);
    }
    
    
    
    public void SetAllowToThrow(bool state) {
        _allowToThrow = state;
        _throwVisualize.SetActiveTrajectoryVisual(state);
        // поведение бота, както сымитировать бросок
        if (_allowToThrow == state && !PlayerHandle) {
            
        }
    }
}
