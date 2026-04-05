using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


[Serializable]
public enum TaskType {
   HitCount,
   WinCount,
   UseHealCount,
   ShieldCount,
   HealsLifesCount,
   Parkour,
}

[Serializable]
public class TaskInfo {
    public float FullValue;
    public int TaskMoney;
    public TaskType TaskType;
}


public class TasksManager : MonoBehaviour {
    [Header("Набор заданий")]
    [SerializeField] private List<TaskInfo> _tasksInfo;
    
    [Header("Визуалы")]
    [SerializeField] private List<TaskVisual> _tasksVisual;    
    [Header("Визуалы")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _openCanvasButton;
    [SerializeField] private Button _closeCanvasButton;
    [Header("Синглтоны")]
    [SerializeField] private ParkourCompleteTrigger _parkourCompleteTrigger;
    
    
    // Инфа по заданию и росту
    private readonly Dictionary<TaskType, TaskInfo> _taskTypeToInfoDictionary = new ();
    private readonly Dictionary<TaskType, TaskVisual> _taskTypeToVisualDictionary = new ();
        

    // Стата игрока в данный момент 
    private int _hitCount;
    private int _parkour;
    private int _winCount;
    private int _useHealCount;
    private int _shieldCount;
    private int _healsLifesCount;
  
    public event Action TaskComplete;
    

    [Inject] private PlayerMovement _playerMovement;
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private PlayerBank _bank;
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationData _localization; 
    [Inject] private BattleManager _battleManager; 
    [Inject] private ThrowGameStarter _throwGameStarter; 
    [Inject] private GameOverShower _gameOverShower; 
    [Inject] private HpView _hpSystem; 
    
    
    
    private void Awake() {
        CreateTaskInfoDictionary();
        CreateTaskVisualDictionary();
    }

    private void OnEnable() {
        _openCanvasButton.onClick.AddListener(() => _canvas.ActiveSelf());
        _closeCanvasButton.onClick.AddListener(() => _canvas.DisactiveSelf());
        
        
        _hpSystem.PlayerHit += UpdatePlayerHit;
        _parkourCompleteTrigger.ParkourCompleted += UpdateParkourTask;
        _gameOverShower.PlayerWon += PlayerWinCheck;
    }

    private void PlayerWinCheck(bool winner) {
        if (winner) {
            _winCount++;
            UpdateTaskProgress(TaskType.WinCount);
        }
        
    }

    private void UpdateParkourTask() {
        _parkour = 1;
        UpdateTaskProgress(TaskType.Parkour);
    }

    private void UpdatePlayerHit() {
        if (_battleManager.MainPlayerPlay && _battleManager.IsFirstThrowerStep) {
            _hitCount++;
            UpdateTaskProgress(TaskType.HitCount);
        }
    }

    private void Start() {
        TableInitialize();
        _parkourCompleteTrigger.SetParkourRewardText(_taskTypeToInfoDictionary[TaskType.Parkour].TaskMoney);
    }
    
    private void TableInitialize() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            TaskInfo taskInfo = _taskTypeToInfoDictionary[taskVisual.Key];
            _taskTypeToVisualDictionary[taskVisual.Key].SetTaskLocalizationText();
            _taskTypeToVisualDictionary[taskVisual.Key].SetTaskVisual(taskInfo.TaskMoney, 0, taskInfo.FullValue);
        }
    }

    private void CreateTaskInfoDictionary() {
        foreach (var task in _tasksInfo) {
            if (_taskTypeToInfoDictionary.ContainsKey(task.TaskType)) {
                Debug.LogWarning($"Повтор ключа! {task.TaskType}");
            }
            _taskTypeToInfoDictionary[task.TaskType] = task;
        }
    }
    
    private void CreateTaskVisualDictionary() {
        foreach (var task in _tasksVisual) {
            if (_taskTypeToVisualDictionary.ContainsKey(task.TaskType)) {
                Debug.LogWarning($"Повтор ключя! {task.TaskType}");
            }
            _taskTypeToVisualDictionary[task.TaskType] = task;
        }
    }

    private int GetPlayerValue(TaskType taskType) {
        switch (taskType) {
            case TaskType.HitCount:
                return _hitCount;
            case TaskType.Parkour:
                return _parkour;
            case TaskType.WinCount:
                return _winCount;
            case TaskType.UseHealCount:
                return _useHealCount;
            case TaskType.ShieldCount:
                return _shieldCount;
            case TaskType.HealsLifesCount:
                return _healsLifesCount;
            default: return -1;
        }
    }

    
    private void UpdateTaskProgress(TaskType type) {
        int currentValue = GetPlayerValue(type);
        TaskInfo taskInfo = _taskTypeToInfoDictionary[type];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[type];
        
        if (currentValue >= taskInfo.FullValue && !taskVisual.TaskIsComplete) {
            taskVisual.SetTaskCompleteVisual(currentValue, taskInfo.FullValue);
            ShowNotification(taskInfo);
        }
        else {
            taskVisual.UpdateTaskScoreVisual(currentValue, taskInfo.FullValue);
        }
    }


    public void SetCompleteTask(TaskType taskType) {
        // Обновляем данные
        TaskInfo taskInfo = _taskTypeToInfoDictionary[taskType];
        _bank.AddMoney(taskInfo.TaskMoney);
    }

    private void ShowNotification(TaskInfo taskInfo) {
        TaskComplete?.Invoke();
        Debug.LogWarning("Таска выполнена!");
        // _taskNotification.ShowNotification("+"+ _formatter.ValuteFormatter(taskInfo.TaskMoney));
    }

}
