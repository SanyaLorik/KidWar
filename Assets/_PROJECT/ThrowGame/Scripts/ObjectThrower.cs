using System;
using System.Collections;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
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
    [field: Header("Сколько держать времени")]

    private bool _allowToThrow;

    public int CurrentLifesCount { get; private set; }

    public Transform ThrowPoint =>  _throwVisualize.ThrowPoint;
    public Transform PointToCameraFocus =>  _throwVisualize.PointToCameraFocus;
    public float AngleToThrow =>  _throwVisualize.CurrentVerticalAngle;

    // Расчет физики полета и тп
    [Inject] private BattleManager _battleManager;
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private HpView _hpView;
    [Inject] private InputThrowGame _inputThrowGame;

    

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
