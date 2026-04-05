using System;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TaskVisual : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _rewardMoneyText;
    [SerializeField] private TextMeshProUGUI _taskText;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private RectTransform _parentRectTransform;
    [SerializeField] private RectTransform _progressRectTransform;
    [field: SerializeField] public TaskType TaskType { get; private set; }
    [SerializeField] private Button _takeRewardButton;
    
    public bool TaskIsComplete { get; private set; }
    private string _taskLocalizationText;
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private LocalizationData _localization;
    [Inject] private TasksManager _tasksManager;


    private void Start() {
        _takeRewardButton.onClick.AddListener(GetTaskRewardByClick);
    }

    private void GetTaskRewardByClick() {
        _tasksManager.SetCompleteTask(TaskType);
        DisableTask();
    }

    
    public void EnableTask(TaskInfo taskInfo) {
        gameObject.ActiveSelf();
        _takeRewardButton.interactable = false;
        SetTaskVisual(taskInfo, 0);
        SetTaskLocalizationText();
    }

    public void DisableTask() {
        gameObject.DisactiveSelf();
        _takeRewardButton.interactable = false;
    }

    
    public void SetTaskLocalizationText() {
        _taskLocalizationText = _localization.GetTranslatedName(TaskType,  _localization.TaskTranslates);
    }

    public void SetTaskVisual(TaskInfo taskInfo, int playerValue) {
        _rewardMoneyText.text = _formatter.ValuteFormatter(taskInfo.TaskMoney);
        _taskText.text = string.Format(_taskLocalizationText, _formatter.ValuteFormatter(taskInfo.FullValue));

        if (playerValue >= taskInfo.FullValue) {
            _takeRewardButton.interactable = true;
            TaskIsComplete = true;
        }
        else {
            _takeRewardButton.interactable = false;
            TaskIsComplete = false;
        }
        UpdateTaskScoreVisual(playerValue, taskInfo.FullValue);
    }

    public void UpdateTaskScoreVisual(float currentValue, float fullValue) {
        Debug.Log($"Обновление значения задачи {TaskType}:  {currentValue}/{fullValue}");
        float percent = Math.Min(currentValue, fullValue) / fullValue;
        _progressText.text = $"{Math.Min(currentValue, fullValue)}/{fullValue}";
        
        RectTransformHelper.SetFillAmount(_progressRectTransform, _parentRectTransform, percent);
    }

    
    public void SetTaskCompleteVisual(float currentValue, float fullValue) {
        _takeRewardButton.interactable = true;
        _progressText.text = $"{Math.Min(currentValue, fullValue)}/{fullValue}";
        Debug.Log($"Обновление значения задачи {TaskType}:  {currentValue}/{fullValue}");
        RectTransformHelper.SetFillAmount(_progressRectTransform, _parentRectTransform, 1);
        TaskIsComplete = true;
    }

    
}
