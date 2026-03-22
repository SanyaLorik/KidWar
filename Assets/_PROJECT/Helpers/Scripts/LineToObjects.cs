using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class LineToObjects : MonoBehaviour {
    // [SerializeField] private Transform _lineTransform; // - 3.33
    // [SerializeField] private int _countTimesShowLine;
    // [SerializeField] private Transform _posForBoost; // -2.76
    // [SerializeField] private Transform _posForSpawn; // - 3.33
    // [SerializeField] private LineRenderer _lineRenderer;
    // [SerializeField] private float _speed = 1f;
    // [SerializeField] private PairedValue<AnimationCurve> _sizeDiapasoneCurves;
    // [SerializeField] private PairedValue<Vector2> _tileDiapasone;
    //
    // [Header("Дистанция для скрытия если пролетел буст")]
    // [SerializeField] private float _distanceToHideArrow = 50f;
    //
    //
    // private Vector3 _target;
    // private float _offset;
    //
    // private bool _arrowInBoost;
    //
    // [Inject] private PlayerStateManager _playerStateManager;
    // [Inject] private PlayerMovement _player;
    // [Inject] private ZoneManager _zoneManager;
    // [Inject] private BoostSpawner _boostSpawner;
    //
    // [Inject] private TasksManager _tasksManager;
    // [Inject] private TutorialCompiller _tutorialCompiller;
    // [Inject] private LevelBounds _levelBounds;
    //
    // private bool TutorialStarted => !_tutorialCompiller.TutorialPassed; 
    //
    // private void OnEnable() {
    //     _player.SetBoost += PlayerOnSetBoost;
    //     _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    //     _zoneManager.ChooseMultiplier += ChooseMultiplier;
    //     if (!_tutorialCompiller.TutorialPassed) {
    //         _tutorialCompiller.TutorialIsOver += TutorialEnd;
    //     }
    // } 
    //
    // private void Start() {
    //     SetSpawnPose();
    // }
    //     
    //
    // private void Update() {
    //     if (_target != Vector3.zero) {
    //         // Обновляем позиции линии
    //         _lineRenderer.SetPosition(0, _lineTransform.position); // от игрока
    //         _lineRenderer.SetPosition(1, _target); // до цели
    //         if (_player.transform.position.z > _target.z+_distanceToHideArrow && _arrowInBoost) {
    //             ForceHideArrow();
    //         }
    //         _offset += Time.deltaTime * _speed;
    //         _lineRenderer.material.mainTextureOffset = new Vector2(_offset, 0);
    //     }
    // }
    //
    // private void PlayerStateManagerOnChangeState(PlayerState state) {
    //     if (state == PlayerState.Walking) {
    //         SetSpawnPose();
    //         if (_arrowInBoost) {
    //             _arrowInBoost = false;
    //         }
    //         _tasksManager.CheckToNeedLine();
    //     }
    //
    //     else if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
    //         _currentShowLine = 0;
    //         HideArrow();
    //     }
    // }
    //
    // // Метод для изменения цели
    // public void SetTarget(Vector3 newTarget) {
    //     Debug.Log("SetTarget " + newTarget);
    //     if (!_arrowInBoost && !_tutorialCompiller.TutorialPassed) {
    //         return;
    //     }
    //     _target = newTarget;
    //     _lineRenderer.enabled = (_target != Vector3.zero);
    //     if (_target != Vector3.zero) {
    //         _lineTransform.ActiveSelf();
    //     }
    //     else {
    //         HideArrow();
    //     }
    // }
    //
    //
    // private void TutorialEnd() {
    //     _tasksManager.CheckToNeedLine();
    // }
    //
    // public void SetTargetTutorial(Vector3 newTarget) {
    //     Debug.Log("SetTargetTutorial " + newTarget);
    //     _arrowInBoost = false;
    //     _target = newTarget;
    //     _lineRenderer.enabled = (_target != Vector3.zero);
    //     _lineTransform.ActiveSelf();
    //     ResetOffset();
    // }
    //
    // public void HideArrow() {
    //     if(TutorialStarted) return;
    //     _target = Vector3.zero;
    //     _lineTransform.DisactiveSelf();
    //     ResetOffset();
    // }
    //
    // private void ForceHideArrow() {
    //     Debug.Log("ForceHideArrow");
    //     _target = Vector3.zero;
    //     _lineTransform.DisactiveSelf();
    //     ResetOffset();
    // }
    //
    // private void ResetOffset() {
    //     _offset = 0f; // сброс
    //     _lineRenderer.material.mainTextureOffset = Vector2.zero;
    // }
    //
    //
    // private void ChooseMultiplier(float obj) {
    //     if(TutorialStarted) return;
    //     HideArrow();
    // }
    //
    // private int _currentShowLine;
    // private void PlayerOnSetBoost() {
    //     if (_tutorialCompiller.TutorialPassed) {
    //         if (_currentShowLine == _countTimesShowLine || Mathf.Approximately(_player.TargetPos.y, _boostSpawner.YMinBoost)) {
    //             HideArrow();
    //             return;
    //         }
    //     }
    //     else {
    //         if (Mathf.Approximately(_player.TargetPos.y, _boostSpawner.YMinBoost)) {
    //             HideArrow();
    //             return;
    //         }
    //     }
    //     SetBoosterPose();
    //     _arrowInBoost = true;
    //     _currentShowLine++;
    //     SetTarget(_player.TargetPos);
    // }
    //
    //
    //
    // private void SetBoosterPose() {
    //     _lineTransform.localPosition = _posForBoost.localPosition;
    //     _lineRenderer.widthCurve = _sizeDiapasoneCurves.To;
    //     _lineRenderer.material.mainTextureScale = new Vector2(_tileDiapasone.To.x, _tileDiapasone.To.y);
    // }
    //
    // private void SetSpawnPose() {
    //     _lineTransform.localPosition = _posForSpawn.localPosition;
    //     _lineRenderer.widthCurve = _sizeDiapasoneCurves.From;
    //     _lineRenderer.material.mainTextureScale = new Vector2(_tileDiapasone.From.x, _tileDiapasone.From.y);
    // }
       
    
}
