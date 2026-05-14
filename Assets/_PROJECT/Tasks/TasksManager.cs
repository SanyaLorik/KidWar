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
    public string TaskId;
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
    private readonly Dictionary<string, TaskInfo> _taskIdToInfoDictionary = new ();
    private readonly Dictionary<string, TaskVisual> _taskVisualIdToViewDictionary = new ();
        
    
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
        TableInitialize();
        if (_dailyQuest.IsTimePassed) {
            ResetCompletedTasks();
        }
        CheckTaskCount();
    }


    private void ShowAdv() {
        _advertisingMonetization.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    // Debug.Log("Обновление тасок");
                    ResetCompletedTasks();
                }
            }
        );
    }
    
    private void ResetCompletedTasks() {
        _dailyQuest.ShowDailies();
        foreach (var taskVisual in _taskVisualIdToViewDictionary) {
            // Debug.Log(taskVisual.Value.TaskId);
            if (Saver.GetTaskInfo(taskVisual.Value.TaskId).IsGetReward) {
                TaskInfo taskInfo = _taskIdToInfoDictionary[taskVisual.Value.TaskId];
                LoadPlayerValue(taskVisual.Value.TaskType, 0, taskVisual.Value.TaskId);
                ResetTaskById(taskVisual);
                taskVisual.Value.EnableTask(taskInfo);
            }
        }
        _gameSave.Save();
    }

    
    private void ResetTaskById(KeyValuePair<string, TaskVisual> taskVisual) {
        Saver.UpdateTaskInfo(taskVisual.Value.TaskId, 0, false);
    }


    private void OnPlayerShielded() {
        if (_battleManager.PlayerStepInPvb) {
            ++_shieldCount;
            // Debug.Log($"Игрок использовал щит {_shieldCount} раз");
            UpdateTaskProgress(TaskType.ShieldCount);
        }        
    }

    
    private void OnPlayerHeal(int count) {
        ++_useHealCount;
        UpdateTaskProgress(TaskType.UseHealCount);

        _healsLifesCount += count;
        UpdateTaskProgress(TaskType.HealsLifesCount);
        // Debug.Log($"Игрок использовал аптечку  {_useHealCount} раз, излечил {_healsLifesCount} здоровья");
    }

    
    private void PlayerWinCheck(bool winner) {
        if (winner) {
            ++_winCount;
            UpdateTaskProgress(TaskType.WinCount);
            // Debug.Log($"Игрок выиграл {_winCount} раз");
        }
    }

    
    private void UpdateParkourTask() {
        _parkour++;
        UpdateTaskProgress(TaskType.Parkour);
        // Debug.Log($"Игрок прошел паркур");
    }


    private void UpdatePlayerHit() {
        if (_battleManager.MainPlayerPlay && _battleManager.IsFirstThrowerStep) {
            ++_hitCount;
            UpdateTaskProgress(TaskType.HitCount);
            // Debug.Log($"Игрок попал {_hitCount} раз");
        }
    }
    
    
    
    private void TableInitialize() {
        int iterator = 0;
        foreach (var taskInfoPair in _taskIdToInfoDictionary) {
            // Initialize
            TaskInfo taskInfo = taskInfoPair.Value;
            string taskId = taskInfo.TaskId;
            _taskVisualIdToViewDictionary[taskId].InitTask(taskId, taskInfo.TaskType);
            
            // Get save data
            TaskItem taskSaveInfo = Saver.GetTaskInfo(taskId);
            
            if (!taskSaveInfo.IsGetReward) {
                _taskVisualIdToViewDictionary[taskId].SetTaskVisual(taskInfo, taskSaveInfo.Count);
                if (taskSaveInfo.Count >= taskInfo.FullValue) {
                    _taskCountView.PlusOne();
                }
                LoadPlayerValue(taskInfo.TaskType, taskSaveInfo.Count, taskInfo.TaskId);
            }
            else {
                // Debug.Log($"Задача {taskSaveInfo.Id} загрузилась как выполненная");
                Debug.Log($"Задача {taskInfo.TaskId} загрузилась как выполненная, вырубаем");
                _taskVisualIdToViewDictionary[taskId].DisableTask();
            }
        }
    }

   

    private void CreateTaskInfoDictionary() {
        int iterator = 0;
        foreach (var task in _tasksInfo) {
            if (_taskIdToInfoDictionary.ContainsKey(task.TaskId)) {
                // Debug.LogWarning($"Повтор ключа! {task.TaskType}");
                continue;
            }
            _taskIdToInfoDictionary[task.TaskId] = task;
            _taskVisualIdToViewDictionary[task.TaskId] = _tasksVisual[iterator++]; 
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
            default: return 0;
        }
    }

    private void LoadPlayerValue(TaskType taskType, int count, string id) {
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
        foreach (var taskVisualPair in _taskVisualIdToViewDictionary) {
            int currentValue = GetPlayerValue(type);
            TaskInfo taskInfo = _taskIdToInfoDictionary[taskVisualPair.Value.TaskId];
            
            if(taskVisualPair.Value.TaskType != type) continue;
            
            TaskVisual taskVisual = taskVisualPair.Value;
            
            if (taskVisual.TaskIsComplete) continue;
            
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
            Saver.UpdateTaskInfo(taskInfo.TaskId, currentValue, false );
            _gameSave.Save();
        }
    }
    

    public void SetCompleteTask(string taskId) {
        // Обновляем данные
        TaskInfo taskInfo = _taskIdToInfoDictionary[taskId];
        Saver.UpdateTaskInfo(taskInfo.TaskId, taskInfo.FullValue, true);
        _bank.AddMoney(taskInfo.TaskMoney);
        _taskCountView.MinusOne();
        CheckTaskCount();
    }
    
    

    private void CheckTaskCount() {
        foreach (var taskVisual in _taskVisualIdToViewDictionary) {
            TaskInfo taskInfo = _taskIdToInfoDictionary[taskVisual.Value.TaskId];
            
            TaskItem taskSaveInfo = Saver.GetTaskInfo(taskInfo.TaskId);
            if(!taskSaveInfo.IsGetReward) return;
        }
        _dailyQuest.ShowAllDaliesDone();
    }

    private void ShowNotification(TaskInfo taskInfo) {
        TaskComplete?.Invoke();
        // Debug.LogWarning("Таска выполнена!");
        // _taskNotification.ShowNotification("+"+ _formatter.ValuteFormatter(taskInfo.TaskMoney));
    }

}