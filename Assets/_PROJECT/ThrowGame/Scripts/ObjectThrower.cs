using UnityEngine;
using Zenject;


/// <summary>
/// Делает бросок с помощью ObjectThrowerBase
/// </summary>
public class ObjectThrower : MonoBehaviour {
    [SerializeField] private PlayerHp _playerHp;
    [SerializeField] private HitPositionCalculator _hitCalculator; 
    
    [field: SerializeField] public Transform PointToBeat { get; private set; }
    [field: SerializeField] public bool StayInLeft { get; private set; }

    [field: SerializeField] public TrajectoryVisualize3D ThrowVisualize { get; private set; }

    [field: Header("Игрок или бот управляет")]
    [field: SerializeField] public bool PlayerHandle { get; set; }

    [field: Header("Сколько держать времени")]

    private bool _allowToThrow;

    public int CurrentLifesCount => _playerHp.CurrentHp;

    public bool IsShielded => _playerHp.IsShielded;

    public Transform ThrowPoint =>  ThrowVisualize.ThrowPoint;

    public IDamageable Damageable => _playerHp;
    public string Nickname { get;  private set; }
    
    public float AngleToThrow { get; private set; }

    private BotObjectThrower _botObjectThrower;

    private void Start() {
        SetHitCalculatorState(false);
    }

    // Расчет физики полета и тп
    [Inject] private BattleManager _battleManager;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private InputThrowGame _inputThrowGame;
    [Inject] private GameData _gameData;

        
    private void OnEnable() {
        _inputThrowGame.OnUpped += Throw;
    }

    public void SetNickname(string nickname) {
        Nickname = nickname;
    }
    
    public void SetBotBehaviour(BotObjectThrower botObjectThrower, ObjectThrower enemyObjectThrower, bool isBot) {
        _botObjectThrower = botObjectThrower;
        _botObjectThrower.SetData(this, enemyObjectThrower);
        PlayerHandle = !isBot;
    }

    
    public void SetHitCalculatorState(bool state) {
        _hitCalculator.enabled = state;
        Debug.Log("SetHitCalculatorState = "+ state);
    }

    public void SetAllowToCalculateHit(bool state) {
        _hitCalculator.SetCalculateState(state);
        Debug.Log("SetAllowToCalculateHit = "+ state);
    }
    
    
    public string SkinId { get; private set; }
    public void InitToNewGame(bool stayInLeft, string skinId) {
        SkinId = skinId;
        StayInLeft = stayInLeft;
        _playerHp.InitPosition(StayInLeft);
        _playerHp.SetMaxHp();
        _playerHp.DisableShield();
    }

    public void SetDead() {
        _playerHp.SetDead();
    }


    public void SetUnshielded() {
        _playerHp.DisableShield();
    }

    private void Throw() {
        // Нажали
        if (_allowToThrow && !_calculator.ObjectInFly) {
            AngleToThrow = ThrowVisualize.CurrentVerticalAngle;
            Debug.Log("ВЫСТРЕЛ!");
            _battleManager.DoStep(this);
        }
    }

    public void BotThrow(float newAngleToThrow) {
        AngleToThrow = newAngleToThrow;
        // Debug.Log("ВЫСТРЕЛ БОТА!");
        _battleManager.DoStep(this);
    }

    
    
    public void SetAllowToThrow(bool state) {
        ThrowVisualize.SetAllowToChooseAngle(state);
        // Debug.Log("SetAllowToThrow " + state);
        _allowToThrow = state;
        // Сам по себе не бьёт

        // поведение бота, както сымитировать бросок
        if (_allowToThrow && !PlayerHandle) {
            // Запрещаем игроку и даем боту
            _allowToThrow = false;
            _botObjectThrower.TransferControl();
        }
    }
}