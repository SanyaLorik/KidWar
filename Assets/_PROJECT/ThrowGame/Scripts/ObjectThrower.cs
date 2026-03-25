using System.Collections;
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

    private bool _isCharging = false;
    private float _chargeTime = 0f;
    private float _requiredHoldTime = 0.5f; // Сколько нужно держать

    void Update() 
    {
        // Нажали
        if (_mouse.leftButton.wasPressedThisFrame) 
        {
            Debug.Log($"Нажали! _allowToThrow = {_allowToThrow}");
            if (_allowToThrow) 
            {
                _isCharging = true;
                _chargeTime = 0f;
                Debug.Log("Начали зарядку");
            }
        }

        // Держим
        if (_isCharging && _mouse.leftButton.isPressed) 
        {
            _chargeTime += Time.deltaTime;
            Debug.Log($"Зарядка: {_chargeTime}/{_requiredHoldTime}");
        }

        // Отпустили
        if (_mouse.leftButton.wasReleasedThisFrame) 
        {
            Debug.Log($"Отпустили! _isCharging = {_isCharging}, _chargeTime = {_chargeTime}, _allowToThrow = {_allowToThrow}");
        
            if (_isCharging && _allowToThrow) 
            {
                _isCharging = false;
            
                if (_chargeTime >= _requiredHoldTime) 
                {
                    Debug.Log("ВЫСТРЕЛ!");
                    _battleManager.DoStep(this);
                }
                else
                {
                    Debug.Log($"Зарядка не завершена ({_chargeTime} < {_requiredHoldTime})");
                }
            }
        }
    }
    

    public void SetStartHp(int hp) {
        CurrentLifesCount = hp;
        _hpView.ChangeHp(CurrentLifesCount, _stayInLeft);
    }
    
    public void MinusHp(int hp) {
        if (IsInvinsible) {
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
        _isCharging = false;
        _chargeTime = 0f;
        _throwVisualize.SetActiveTrajectoryVisual(state);
        // поведение бота, както сымитировать бросок
        if (_allowToThrow == state && !PlayerHandle) {
            
        }
    }

    public bool IsInvinsible { get; private set; }

    public void SetInvinsible(bool state) {
        IsInvinsible = state;
    }
}
