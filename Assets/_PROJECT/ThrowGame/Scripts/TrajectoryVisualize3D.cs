using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


/// <summary>
/// Также может как бот выбирать так и игрок
/// </summary>
public class TrajectoryVisualize3D : MonoBehaviour
{
    [Header("Ссылки")]
    [field: SerializeField] public Transform ThrowPoint { get; private set; }
    
    [SerializeField] private LineRenderer _trajectoryLine; // Линия траектории
    
    [Header("Ограничения угла")]
    [SerializeField] private float maxUpAngle = 90f;      // Максимальный угол вверх
    [SerializeField] private float maxDownAngle = 45f;    // Максимальный угол вниз
    
    
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _lineLength;

    private Vector3 _throwDirection;
    
    public float CurrentVerticalAngle { get; private set; }
    public bool AllowToChooseAngle { get; private set; }

    [Inject] private InputThrowGame _inputThrowGame;
    [Inject] private ObjectThrowerCalculator _calculator;


    
    private void Start() {
        _trajectoryLine.DisactiveSelf();
    }

    
    private void OnEnable() {
        _inputThrowGame.OnDragged += DragController;
        _inputThrowGame.OnUpped += OnUppedScreen;
        _inputThrowGame.OnDowned += OnDownedScreen;
    }
    
    
    private void OnDisable() {
        _inputThrowGame.OnDragged -= DragController;
        _inputThrowGame.OnUpped -= OnUppedScreen;
        _inputThrowGame.OnDowned -= OnDownedScreen;
    }
    
    public void InitCurrentAngleByBot(float angle) {
        CurrentVerticalAngle = angle;
    }

    public void SetAllowToChooseAngle(bool state) {
        AllowToChooseAngle = state;
        _trajectoryLine.gameObject.SetActive(state);
        if (state) {
            UpdateThrowDirectionAndDraw();
        }
    }
    
    
    private void OnUppedScreen() {
        if (!AllowToChooseAngle) return;
        _trajectoryLine.gameObject.DisactiveSelf();
    }
    
    
    private void OnDownedScreen() {
        if (!AllowToChooseAngle) return;
        _trajectoryLine.gameObject.ActiveSelf();
    }

    public void SetActiveTrajectoryLine(bool state) {
        _trajectoryLine.gameObject.SetActive(state);
    }

    
    private void DragController(Vector2 delta) {
        if (!AllowToChooseAngle) return;
        if(_calculator.ObjectInFly) return;
        UpdateThrowDirection(delta);
        DrawTrajectory();
    }

    

    private void UpdateThrowDirection(Vector2 screenDelta) {
        int sign = transform.forward.z > 0 ? 1 : -1;
        // Накопление угла
        CurrentVerticalAngle += screenDelta.y * _sensitivity;
        CurrentVerticalAngle = Mathf.Clamp(CurrentVerticalAngle, -maxDownAngle, maxUpAngle);
        // Debug.Log("Угол: " + CurrentVerticalAngle);
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
    
    [Header("Анимация выбора угла")]
    [SerializeField] private Vector2 _overshootOffset = new Vector2(15f, 30f);
    [SerializeField] private Vector2 _undershootOffset = new Vector2(10f, 25f);
    [SerializeField] private Vector2 _phase1DurationRange = new Vector2(0.3f, 0.5f);
    [SerializeField] private Vector2 _phase2DurationRange = new Vector2(0.2f, 0.4f);
    [SerializeField] private Vector2 _phase3DurationRange = new Vector2(0.2f, 0.4f);

    [Header("Вариативность поведения")]
    [SerializeField] private float _smoothVariationChance = 0.3f;
    [SerializeField] private float _onlyOvershootChance = 0.2f;
    [SerializeField] private float _onlyUndershootChance = 0.2f;

    public async UniTask SelectAngleWithAnimationAsync(float targetAngle, float totalDuration, CancellationToken token) {
        float startAngle = CurrentVerticalAngle;
        
        // Рандомно выбираем паттерн поведения
        float randomPattern = Random.value;
        float smoothThreshold = _smoothVariationChance;
        float overshootThreshold = smoothThreshold + _onlyOvershootChance;
        float undershootThreshold = overshootThreshold + _onlyUndershootChance;
        
        if (randomPattern < smoothThreshold) {
            // Паттерн 1: Плавный выбор
            await AnimateAngleSimple(startAngle, targetAngle, totalDuration, token);
            return;
        }

        float overshootAmount;
        float undershootAmount;
        if (randomPattern < overshootThreshold) {
            // Паттерн 2: Только перелет
            overshootAmount = Random.Range(_overshootOffset.x, _overshootOffset.y);
            float overshootValue = targetAngle + overshootAmount;
            overshootValue = Mathf.Clamp(overshootValue, -maxDownAngle, maxUpAngle);
            await AnimateAngleSimple(startAngle, overshootValue, totalDuration * 0.6f, token);
            await AnimateAngleSimple(overshootValue, targetAngle, totalDuration * 0.4f, token);
            return;
        }
        
        if (randomPattern < undershootThreshold) {
            // Паттерн 3: Только недолет
            undershootAmount = Random.Range(_undershootOffset.x, _undershootOffset.y);
            float undershootValue = targetAngle - undershootAmount;
            undershootValue = Mathf.Clamp(undershootValue, -maxDownAngle, maxUpAngle);
            await AnimateAngleSimple(startAngle, undershootValue, totalDuration * 0.6f, token);
            await AnimateAngleSimple(undershootValue, targetAngle, totalDuration * 0.4f, token);
            return;
        }
        
        // Паттерн 4: Классический перелет + недолет
        overshootAmount = Random.Range(_overshootOffset.x, _overshootOffset.y);
        undershootAmount = Random.Range(_undershootOffset.x, _undershootOffset.y);
        
        float overshootAngle = Mathf.Clamp(targetAngle + overshootAmount, -maxDownAngle, maxUpAngle);
        float undershootAngle = Mathf.Clamp(targetAngle - undershootAmount, -maxDownAngle, maxUpAngle);
        
        float phase1Ratio = Random.Range(_phase1DurationRange.x, _phase1DurationRange.y);
        float phase2Ratio = Random.Range(_phase2DurationRange.x, _phase2DurationRange.y);
        float phase3Ratio = 1f - phase1Ratio - phase2Ratio;
        
        phase3Ratio = Mathf.Clamp(phase3Ratio, _phase3DurationRange.x, _phase3DurationRange.y);
        
        float phase1Duration = totalDuration * phase1Ratio;
        float phase2Duration = totalDuration * phase2Ratio;
        float phase3Duration = totalDuration * phase3Ratio;
        
        await AnimateAngleSimple(startAngle, overshootAngle, phase1Duration, token);
        await AnimateAngleSimple(overshootAngle, undershootAngle, phase2Duration, token);
        await AnimateAngleSimple(undershootAngle, targetAngle, phase3Duration, token);
    }

    private async UniTask AnimateAngleSimple(float from, float to, float duration, CancellationToken token) {
        if (duration <= 0) {
            CurrentVerticalAngle = to;
            UpdateThrowDirectionAndDraw();
            return;
        }
        
        float elapsed = 0f;
        while (elapsed < duration && !token.IsCancellationRequested) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // Простой SmoothStep без рандома
            float easedT = Mathf.SmoothStep(0, 1, t);
            CurrentVerticalAngle = Mathf.Lerp(from, to, easedT);
            UpdateThrowDirectionAndDraw();
            await UniTask.Yield();
        }
        
        if (!token.IsCancellationRequested) {
            CurrentVerticalAngle = to;
            UpdateThrowDirectionAndDraw();
        }
    }

    private void UpdateThrowDirectionAndDraw() {
        int sign = transform.forward.z > 0 ? 1 : -1;
        Vector3 throwBaseDirection = transform.forward;
        throwBaseDirection.y = 0;
        throwBaseDirection.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(-CurrentVerticalAngle * sign, Vector3.right);
        _throwDirection = rotation * throwBaseDirection;
        DrawTrajectory();
    }
    
}