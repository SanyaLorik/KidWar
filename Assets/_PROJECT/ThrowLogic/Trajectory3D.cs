using UnityEngine;
using UnityEngine.InputSystem;

public class Trajectory3D : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform throwPoint;     // Точка вылета снаряда (пустой GameObject)
    [SerializeField] private LineRenderer trajectoryLine; // Линия траектории
    
    [Header("Настройки")]
    [SerializeField] private float throwForce = 15f;      // Сила броска
    [SerializeField] private int trajectoryPoints = 50;   // Количество точек траектории
    [SerializeField] private float timeStep = 0.05f;      // Шаг времени
    
    [Header("Ограничения угла")]
    [SerializeField] private float maxUpAngle = 80f;      // Максимальный угол вверх
    [SerializeField] private float maxDownAngle = 45f;    // Максимальный угол вниз
    
    private Camera mainCamera;
    private Vector3 throwDirection;
    private Mouse mouse; // Для New Input System
    
    private float _currentVerticalAngle = 0f;
    [SerializeField] private float _sensitivity = 1f;


    [SerializeField] private float _lineLength;
    
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        // Получаем доступ к мыши через New Input System
        mouse = Mouse.current;
        
        // Настройка LineRenderer
        if (trajectoryLine != null)
        {
            trajectoryLine.startColor = Color.yellow;
            trajectoryLine.endColor = Color.red;
        }
        else
        {
            Debug.LogError("Trajectory Line не назначен!");
        }
        
        if (throwPoint == null)
        {
            Debug.LogError("Throw Point не назначен!");
        }
    }
    
    void Update()
    {
        // Проверяем, что мышь доступна
        if (mouse == null) return;
        
        // Получаем направление от точки броска к мыши
        UpdateThrowDirection();
        
        // Рисуем траекторию
        DrawTrajectory();
    }

    void UpdateThrowDirection()
    {
        // Получаем движение мыши по вертикали
        Vector2 mouseDelta = mouse.delta.ReadValue();
    
        // Накопление угла (как в старых играх)
        _currentVerticalAngle += mouseDelta.y * _sensitivity;
        _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, -maxDownAngle, maxUpAngle);
    
        // Направление вперед от игрока
        Vector3 throwBaseDirection = transform.forward;
        throwBaseDirection.y = 0;
        throwBaseDirection.Normalize();
    
        // Поворачиваем вокруг правого вектора игрока (или глобальной оси)
        Quaternion rotation = Quaternion.AngleAxis(-_currentVerticalAngle, Vector3.right);
        throwDirection = rotation * throwBaseDirection;
    }


    void DrawTrajectory()
    {
        if (trajectoryLine == null) {
            Debug.Log("trajectoryLine = null");
            return;
        }
    
    
        // Конечная точка: startPos + направление * длина
        Vector3 endPos = throwPoint.position + throwDirection * _lineLength;
    
        // Просто рисуем прямую линию из 2 точек
        trajectoryLine.positionCount = 2;
        trajectoryLine.SetPosition(0, throwPoint.position);
        trajectoryLine.SetPosition(1, endPos);
    }
    
}