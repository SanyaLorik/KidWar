using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trajectory3D : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform throwPoint;     // Точка вылета снаряда (пустой GameObject)
    [SerializeField] private LineRenderer trajectoryLine; // Линия траектории
    
    [Header("Ограничения угла")]
    [SerializeField] private float maxUpAngle = 90f;      // Максимальный угол вверх
    [SerializeField] private float maxDownAngle = 45f;    // Максимальный угол вниз
    
    
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _lineLength;
    
    private Camera mainCamera;
    private Vector3 throwDirection;
    private Mouse mouse;


    public event Action<float> AngleChoosed;
    
    public float CurrentVerticalAngle { get; private set; }
    
    private void Start() {
        trajectoryLine.gameObject.ActiveSelf();
        if (mainCamera == null)
            mainCamera = Camera.main;
        // Получаем доступ к мыши через New Input System
        mouse = Mouse.current;
    }
    
    private void Update() {
        if (mouse == null) return;
        
        UpdateThrowDirection();
        DrawTrajectory();
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