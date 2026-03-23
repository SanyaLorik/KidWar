using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrajectoryVisualize3D : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform throwPoint;     // Точка вылета снаряда (пустой GameObject)
    [SerializeField] private LineRenderer trajectoryLine; // Линия траектории
    
    [Header("Ограничения угла")]
    [SerializeField] private float maxUpAngle = 90f;      // Максимальный угол вверх
    [SerializeField] private float maxDownAngle = 45f;    // Максимальный угол вниз
    
    
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _lineLength;
    [SerializeField] private GameObject _trajectoryCanvas;


    private Camera mainCamera;
    private Vector3 throwDirection;
    private Mouse mouse = Mouse.current;
    

    public event Action<float> AngleChoosed;
    
    public float CurrentVerticalAngle { get; private set; }
    
    private void OnEnable() {
        
    }
    private void Start() {
        mainCamera = Camera.main;
        trajectoryLine.gameObject.DisactiveSelf();
    }
    
    private void Update() {
        UpdateThrowDirection();
        DrawTrajectory();
    }
    
    
    private void OnChangeState(PlayerState state) {
        if (state == PlayerState.Play) {
            _trajectoryCanvas.ActiveSelf();
        }
        else {
            _trajectoryCanvas.DisactiveSelf();
        }
    }
 

    private void UpdateThrowDirection() {
        Vector2 mouseDelta = mouse.delta.ReadValue();
        int sign = transform.forward.z > 0 ? 1 : -1;
        // Накопление угла
        CurrentVerticalAngle += mouseDelta.y * _sensitivity;
        CurrentVerticalAngle = Mathf.Clamp(CurrentVerticalAngle, -maxDownAngle, maxUpAngle);
    
        // Направление вперед от игрока
        Vector3 throwBaseDirection = transform.forward;
        throwBaseDirection.y = 0;
        throwBaseDirection.Normalize();
    
        // Поворачиваем вокруг правого вектора игрока (или глобальной оси)
        Quaternion rotation = Quaternion.AngleAxis(-CurrentVerticalAngle * sign, Vector3.right);
        throwDirection = rotation * throwBaseDirection; 
    }


    private void DrawTrajectory() {
        // Конечная точка: startPos + направление * длина
        Vector3 endPos = throwPoint.position + throwDirection * _lineLength;
    
        // Просто рисуем прямую линию из 2 точек
        trajectoryLine.positionCount = 2;
        trajectoryLine.SetPosition(0, throwPoint.position);
        trajectoryLine.SetPosition(1, endPos);
    }
    
}