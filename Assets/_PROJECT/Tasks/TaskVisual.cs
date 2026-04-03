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
    [SerializeField] private Button _takeRewardButton;
    [SerializeField] private RectTransform _parentRectTransform;
    [SerializeField] private RectTransform _progressRectTransform;
    [field: SerializeField] public TaskType TaskType { get; private set; }
    public bool TaskIsComplete { get; private set; }
    private string _taskLocalizationText;
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private LocalizationData _localization;
    [Inject] private TasksManager _tasksManager;


    private void Start() {
        _takeRewardButton.onClick.AddListener(GetTaskRewardByClick);
        _takeRewardButton.DisactiveSelf();
    }

    private void GetTaskRewardByClick() {
        _tasksManager.SetCompleteTask(TaskType);
        _takeRewardButton.DisactiveSelf();
    }

    public void SetTaskLocalizationText() {
        _taskLocalizationText = _localization.GetTranslatedName(TaskType,  _localization.TaskTranslates);
    }

    public void SetTaskVisual(float rewardMoney, float playerValue, float fullValue) {
        _rewardMoneyText.text = _formatter.ValuteFormatter(rewardMoney);
        _taskText.text = string.Format(_taskLocalizationText, _formatter.ValuteFormatter(fullValue));
        
        
        _takeRewardButton.gameObject.DisactiveSelf();
        TaskIsComplete = false;
        UpdateTaskScoreVisual(playerValue, fullValue);
    }

    public void UpdateTaskScoreVisual(float currentValue, float fullValue) {
        Debug.Log($"Обновление значения задачи {TaskType}:  {currentValue}/{fullValue}");
        float percent = Math.Min(currentValue, fullValue) / fullValue;
        _progressText.text = $"{Math.Min(currentValue, fullValue)}/{fullValue}";
        
        RectTransformHelper.SetFillAmount(_progressRectTransform, _parentRectTransform, percent);
    }

    
    public void SetTaskCompleteVisual(float currentValue, float fullValue) {
        _takeRewardButton.gameObject.ActiveSelf();
        _progressText.text = $"{Math.Min(currentValue, fullValue)}/{fullValue}";
        Debug.Log($"Обновление значения задачи {TaskType}:  {currentValue}/{fullValue}");
        RectTransformHelper.SetFillAmount(_progressRectTransform, _parentRectTransform, 1);
        TaskIsComplete = true;
    }

    
}
