using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using LuringPlayer_M;
using MirraSDK_M;
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
    public int FullValue;
    public int TaskMoney;
    public TaskType TaskType;
    public string TaskId => TaskType.ToString();
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
    [SerializeField] private Button _resetButton;

    [Header("Синглтоны")]
    [SerializeField] private ParkourCompleteTrigger _parkourCompleteTrigger;
    [SerializeField] private TaskCompleteCountView _taskCountView;
    [SerializeField] private DailyQuest _dailyQuest;
    
    
    
    // Инфа по заданию и росту
    private readonly Dictionary<TaskType, TaskInfo> _taskTypeToInfoDictionary = new ();
    private readonly Dictionary<TaskType, TaskVisual> _taskTypeToVisualDictionary = new ();
        
    
    // Стата игрока в данный момент 
    private int _hitCount;
    private int _parkour;
    private int _winCount;
    private int _useHealCount;
    private int _healsLifesCount;
    private int _shieldCount;
  
    public event Action TaskComplete;
    private GameSave Saver => _gameSave.GetSave<GameSave>();

    [Inject] private PlayerBank _bank;
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationData _localization; 
    [Inject] private BattleManager _battleManager; 
    [Inject] private ThrowGameStarter _throwGameStarter; 
    [Inject] private GameOverShower _gameOverShower; 
    [Inject] private HpView _hpSystem; 
    [Inject] private IGameSave _gameSave; 
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetization;

    
    private void OnEnable() {
        _openCanvasButton.onClick.AddListener(() => _canvas.ActiveSelf());
        _closeCanvasButton.onClick.AddListener(() => _canvas.DisactiveSelf());
        _resetButton.onClick.AddListener(ShowAdv);
        
        _hpSystem.PlayerHit += UpdatePlayerHit;
        _hpSystem.MainPlayerHeal += OnPlayerHeal;
        _hpSystem.PlayerShielded += OnPlayerShielded;
        _parkourCompleteTrigger.ParkourCompleted += UpdateParkourTask;
        _gameOverShower.PlayerWin += PlayerWinCheck;
        _dailyQuest.OnTimerPassed += ResetCompletedTasks;
    }
    
    
    private void Start() {
        CreateTaskInfoDictionary();
        CreateTaskVisualDictionary();
        TableInitialize();
        if (_dailyQuest.IsTimePassed) {
            ResetCompletedTasks();
        }
    }


    private void ShowAdv() {
        _advertisingMonetization.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    Debug.Log("Обновление тасок");
                    ResetCompletedTasks();
                }
            }
        );
    }
    
    private void ResetCompletedTasks() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            TaskInfo taskInfo = GetTaskInfoByType(taskVisual.Value.TaskType);
            
            if (Saver.GetTaskInfo(taskInfo.TaskId).IsGetReward) {
                SetPlayerValue(taskVisual.Value.TaskType, 0);
                taskVisual.Value.EnableTask(taskInfo);
            }
        }
        _gameSave.Save();
    }

    private TaskInfo GetTaskInfoByType(TaskType taskType)
        => _tasksInfo.First(t => t.TaskType == taskType);
    
    
    private void OnPlayerShielded() {
        if (_battleManager.PlayerStepInPvb) {
            ++_shieldCount;
            Debug.Log($"Игрок использовал щит {_shieldCount} раз");
            UpdateTaskProgress(TaskType.ShieldCount);
        }        
    }

    
    private void OnPlayerHeal(int count) {
        ++_useHealCount;
        UpdateTaskProgress(TaskType.UseHealCount);

        _healsLifesCount += count;
        UpdateTaskProgress(TaskType.HealsLifesCount);
        Debug.Log($"Игрок использовал аптечку  {_useHealCount} раз, излечил {_healsLifesCount} здоровья");
    }

    
    private void PlayerWinCheck(bool winner) {
        if (winner) {
            ++_winCount;
            UpdateTaskProgress(TaskType.WinCount);
            Debug.Log($"Игрок выиграл {_winCount} раз");
        }
    }

    
    private void UpdateParkourTask() {
        _parkour = 1;
        UpdateTaskProgress(TaskType.Parkour);
        Debug.Log($"Игрок прошел паркур");
    }


    private void UpdatePlayerHit() {
        if (_battleManager.MainPlayerPlay && _battleManager.IsFirstThrowerStep) {
            ++_hitCount;
            UpdateTaskProgress(TaskType.HitCount);
            Debug.Log($"Игрок попал {_hitCount} раз");
        }
    }
    
    
    
    private void TableInitialize() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            TaskInfo taskInfo = _taskTypeToInfoDictionary[taskVisual.Key];
            TaskItem taskSaveInfo = Saver.GetTaskInfo(taskInfo.TaskId);
            _taskTypeToVisualDictionary[taskVisual.Key].SetTaskLocalizationText();
            if (!taskSaveInfo.IsGetReward) {
                _taskTypeToVisualDictionary[taskVisual.Key].SetTaskVisual(taskInfo, taskSaveInfo.Count);
                if (taskSaveInfo.Count >= taskInfo.FullValue) {
                    _taskCountView.PlusOne();
                }
                SetPlayerValue(taskInfo.TaskType, taskSaveInfo.Count);
            }
            else {
                Debug.Log($"Задача {taskSaveInfo.Id} загрузилась как выполненная");
                _taskTypeToVisualDictionary[taskVisual.Key].DisableTask();
            }
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

    private void SetPlayerValue(TaskType taskType, int count) {
        string id = _tasksInfo.Find(t => t.TaskType == taskType).TaskId;
        Saver.UpdateTaskInfo(id, count, false);
        switch (taskType) {
            case TaskType.HitCount:
                 _hitCount = count;
                break;
            case TaskType.Parkour:
                 _parkour = count;
                break;
            case TaskType.WinCount:
                _winCount = count;
                break;
            case TaskType.UseHealCount:
                 _useHealCount = count;
                break;
            case TaskType.ShieldCount:
                 _shieldCount = count;
                break;
            case TaskType.HealsLifesCount:
                 _healsLifesCount = count;
                break;
        }
    }

    
    private void UpdateTaskProgress(TaskType type) {
        int currentValue = GetPlayerValue(type);
        TaskInfo taskInfo = _taskTypeToInfoDictionary[type];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[type];
        
        Saver.UpdateTaskInfo(taskInfo.TaskId, currentValue, false );
        _gameSave.Save();
        
        if (currentValue >= taskInfo.FullValue && !taskVisual.TaskIsComplete) {
            taskVisual.SetTaskCompleteVisual(currentValue, taskInfo.FullValue);
            _taskCountView.PlusOne();
            ShowNotification(taskInfo);
        }
        else {
            taskVisual.UpdateTaskScoreVisual(currentValue, taskInfo.FullValue);
        }
    }
    

    public void SetCompleteTask(TaskType taskType) {
        // Обновляем данные
        TaskInfo taskInfo = _taskTypeToInfoDictionary[taskType];
        Saver.UpdateTaskInfo(taskInfo.TaskId, taskInfo.FullValue, true);
        _bank.AddMoney(taskInfo.TaskMoney);
        _taskCountView.MinusOne();
    }

    private void ShowNotification(TaskInfo taskInfo) {
        TaskComplete?.Invoke();
        Debug.LogWarning("Таска выполнена!");
        // _taskNotification.ShowNotification("+"+ _formatter.ValuteFormatter(taskInfo.TaskMoney));
    }

}