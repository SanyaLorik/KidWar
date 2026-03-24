using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Также может как бот выбирать так и игрок
/// </summary>
public class TrajectoryVisualize3D : MonoBehaviour
{
    [Header("Ссылки")]
    [field: SerializeField] public Transform ThrowPoint { get; private set; }
    [field: SerializeField] public Transform PointToCameraFocus { get; private set; }
    
    [SerializeField] private LineRenderer _trajectoryLine; // Линия траектории
    
    [Header("Ограничения угла")]
    [SerializeField] private float maxUpAngle = 90f;      // Максимальный угол вверх
    [SerializeField] private float maxDownAngle = 45f;    // Максимальный угол вниз
    
    
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _lineLength;
    [SerializeField] private GameObject _trajectoryCanvas;


    private Camera _mainCamera;
    private Vector3 _throwDirection;
    // Будет не только мышка нарушка но еще джойстик а еще бот сможет сам выбирать
    private Mouse _mouse = Mouse.current;
    
    
    public float CurrentVerticalAngle { get; private set; }

    private void Start() {
        _mainCamera = Camera.main;
        SetActiveTrajectoryVisual(false);
    }
    
    private void Update() {
        
        UpdateThrowDirection();
        DrawTrajectory();
    }

    public void SetActiveTrajectoryVisual(bool state) {
        _trajectoryCanvas.SetActive(state);
        _trajectoryLine.gameObject.SetActive(state);
    }
    

 

    private void UpdateThrowDirection() {
        Vector2 _mouseDelta = _mouse.delta.ReadValue();
        int sign = transform.forward.z > 0 ? 1 : -1;
        // Накопление угла
        CurrentVerticalAngle += _mouseDelta.y * _sensitivity;
        CurrentVerticalAngle = Mathf.Clamp(CurrentVerticalAngle, -maxDownAngle, maxUpAngle);
    
        // Направление вперед от игрока
        Vector3 throwBaseDirection = transform.forward;
        throwBaseDirection.y = 0;
        throwBaseDirection.Normalize();
    
        // Поворачиваем вокруг правого вектора игрока (или глобальной оси)
        Quaternion rotation = Quaternion.AngleAxis(-CurrentVerticalAngle * sign, Vector3.right);
        _throwDirection = rotation * throwBaseDirection; 
    }


    private void DrawTrajectory() {
        // Конечная точка: startPos + направление * длина
        Vector3 endPos = ThrowPoint.position + _throwDirection * _lineLength;
    
        // Просто рисуем прямую линию из 2 точек
        _trajectoryLine.positionCount = 2;
        _trajectoryLine.SetPosition(0, ThrowPoint.position);
        _trajectoryLine.SetPosition(1, endPos);
    }
    
}