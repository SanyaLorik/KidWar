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
    [Header("Задержка перед выбором")]
    [SerializeField] private PairedValue<float> _delayBeforeUseModifier;
    [SerializeField] private PairedValue<float> _delayBeforeUseBonus;
    [Header("Вероятность что выберет первым модификатор или бонус")]
    [Range(0,1), SerializeField] private float _useModifierFirst;
    
    [SerializeField] private PairedValue<float> _angleChooseTimeDiapasone;
    [SerializeField] private PairedValue<float> _diapasoneNearPlayer;
    
    
    [Inject] private ObjectThrowerCalculator _calculator;
    [Inject] private ForceChooseView _forceView;
    [Inject] private ModifierManager _modifierManager;
    [Inject] private BonusManager _bonusManager;
    
    
    private ObjectThrower _currentThrower;
    private ObjectThrower _enemyThrower;
    private CancellationTokenSource _tokenSource;
    
    public void SetData(ObjectThrower currentThrower, ObjectThrower enemyThrower) {
        _currentThrower = currentThrower;
        _enemyThrower = enemyThrower;
    }

    public void DisposeBot() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
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

        float waitTime;
        // Сначала выбор модификатора 
        if (Random.value <  _useModifierFirst) {
            waitTime = Random.Range(_delayBeforeUseModifier.From, _delayBeforeUseModifier.To);
            await UniTask.WaitForSeconds(waitTime, cancellationToken: _tokenSource.Token);
            _modifierManager.UseModifierForBot();
            
            waitTime = Random.Range(_delayBeforeUseBonus.From, _delayBeforeUseBonus.To);
            await UniTask.WaitForSeconds(waitTime, cancellationToken: _tokenSource.Token);
            _bonusManager.UseBonusForBot();
        }
        else {
            waitTime = Random.Range(_delayBeforeUseBonus.From, _delayBeforeUseBonus.To);
            await UniTask.WaitForSeconds(waitTime, cancellationToken: _tokenSource.Token);
            _bonusManager.UseBonusForBot();
            
            waitTime = Random.Range(_delayBeforeUseModifier.From, _delayBeforeUseModifier.To);
            await UniTask.WaitForSeconds(waitTime, cancellationToken: _tokenSource.Token);
            _modifierManager.UseModifierForBot();
        }
        // Сначала выбор бонуса
        
        
        _currentThrower.ThrowVisualize.SetActiveTrajectoryLine(true);
        float durationToChooseAngle = Random.Range(_angleChooseTimeDiapasone.From, _angleChooseTimeDiapasone.To);

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
        
        _currentThrower.ThrowVisualize.SelectAngleWithAnimationAsync(angle, durationToChooseAngle, _tokenSource.Token).Forget();
        await _forceView.StartBotForceChooser(force, durationToChooseAngle, _tokenSource.Token);

        _currentThrower.ThrowVisualize.SetActiveTrajectoryLine(false);
        _currentThrower.BotThrow(angle);
    } 
}