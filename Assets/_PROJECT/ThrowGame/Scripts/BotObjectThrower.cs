using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


/// <summary>
/// Цель бота решить попадет он в игрока или нет и выбрать для этого силу и угол
/// </summary>
public class BotObjectThrower : MonoBehaviour {
    [Range(0,1), SerializeField] private float _chanceToBeatPlayer;
    [SerializeField] private PairedValue<float> _delayBeforeThrow;
    [SerializeField] private PairedValue<float> _angleChooseTimeDiapasone;
    [SerializeField] private PairedValue<float> _diapasoneNearPlayer;
    
    
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private ForceChooseView _forceView;
    
    private ObjectThrower _currentThrower;
    private ObjectThrower _enemyThrower;
    private CancellationTokenSource _tokenSource;

    
    public void SetData(ObjectThrower currentThrower, ObjectThrower enemyThrower) {
        _currentThrower = currentThrower;
        _enemyThrower = enemyThrower;
    }


    /// <summary>
    /// Передаем управление боту
    /// </summary>
    public void TransferControl() {
        float distance = Vector3.Distance(_currentThrower.PointToBeat.position, _enemyThrower.PointToBeat.position);
        // не попасть по игроку
        if (Random.value > _chanceToBeatPlayer) {
            // тут поидее тоже с прикольчиком будет типо боту слева над знак менять
            distance += Random.Range(_diapasoneNearPlayer.From, _diapasoneNearPlayer.To);
        }
        WaitToForceAsync(distance).Forget();
    }

    private async UniTask WaitToForceAsync(float distance) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        // Ждем время перед выстрелом
        float waitTime = Random.Range(_delayBeforeThrow.From, _delayBeforeThrow.To);
        await UniTask.WaitForSeconds(waitTime);

        _currentThrower.ThrowVisualize.SetActiveTrajectoryLine(true);
        float totalDuration = Random.Range(_angleChooseTimeDiapasone.From, _angleChooseTimeDiapasone.To);

        // Чтобы попасть куда надо подберем значения и инициализируем
        (float force, float angle) force_angle = _calculator.CalculateForceAndAngleToPoint(
            distance, 
            _currentThrower.PointToBeat.position.z, 
            _enemyThrower.PointToBeat.position.z
        );
        
        float angle =  force_angle.angle;
        float force = force_angle.force;
        _forceView.InitBotForce(force);
        _currentThrower.ThrowVisualize.InitCurrentAngleByBot(angle);
        
        _currentThrower.ThrowVisualize.SelectAngleWithAnimationAsync(angle, totalDuration, _tokenSource.Token).Forget();
        await _forceView.StartBotForceChooser(force, totalDuration);

        _currentThrower.ThrowVisualize.SetActiveTrajectoryLine(false);
        _currentThrower.BotThrow(angle);
    } 
}