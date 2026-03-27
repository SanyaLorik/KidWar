using UnityEngine;
using Zenject;


/// <summary>
/// Делает бросок с помощью ObjectThrowerBase
/// </summary>
public class ObjectThrower : MonoBehaviour {
    [SerializeField] private PlayerHp _playerHp;
    [field: SerializeField] public bool StayInLeft { get; private set; }

    [SerializeField] private TrajectoryVisualize3D _throwVisualize;

    [field: Header("Игрок или бот управляет")]
    [field: SerializeField] public bool PlayerHandle { get; set; }

    [field: Header("Сколько держать времени")]

    private bool _allowToThrow;

    public int CurrentLifesCount => _playerHp.CurrentHp;

    public Transform ThrowPoint =>  _throwVisualize.ThrowPoint;
    public Transform PointToCameraFocus =>  _throwVisualize.PointToCameraFocus;
    public float AngleToThrow =>  _throwVisualize.CurrentVerticalAngle;
    
    
    // Расчет физики полета и тп
    [Inject] private BattleManager _battleManager;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private InputThrowGame _inputThrowGame;
    [Inject] private HpView _hpView;
    [Inject] private GameData _gameData;


    public IDamageable Damageable => _playerHp;
    
    
    public void InitToNewGame() {
        _playerHp.InitPosition(StayInLeft);
        _playerHp.SetMaxHp();
    }
    
    
    private void OnEnable() {
        _inputThrowGame.OnUpped += Throw;
    }


    private void Throw() {
        // Нажали
        if (_allowToThrow && !_calculator.ObjectInFly) {
            Debug.Log("ВЫСТРЕЛ!");
            _battleManager.DoStep(this);
        }
    }

    
    public void SetAllowToThrow(bool state) {
        _allowToThrow = state;
        _throwVisualize.SetActiveTrajectoryVisual(state);
        // Сам по себе не бьёт
        _playerHp.SetInvinsible(state);
        // поведение бота, както сымитировать бросок
        if (_allowToThrow == state && !PlayerHandle) {
            
        }
    }
}