using System;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class TaskCompleteCountView : MonoBehaviour {
    [SerializeField] private GameObject _countCompleteTasksContainer;
    [SerializeField] private TextMeshProUGUI _countCompleteTasksText;
    
    
    [Inject] PlayerStateManager _playerStateManager;
    
    private int _countCompleteTasks;

    private void Start() {
        ShowCount();
    }

    private void OnEnable() {
        _playerStateManager.StateChanged += PlayerStateChanged;
    }

    /// <summary>
    /// Игрок вернулся на спавн и показался канвас спавна
    /// </summary>
    /// <param name="state"></param>
    private void PlayerStateChanged(PlayerState state) {
        if (state == PlayerState.InSpawn) {
            ShowCount();
        }
    }


    public void PlusOne() {
        if (!_countCompleteTasksContainer.activeSelf) {
            _countCompleteTasksContainer.ActiveSelf();
        }
        _countCompleteTasks++;
        _countCompleteTasksText.text = _countCompleteTasks.ToString();
    }

    private void ShowCount() {
        if (_countCompleteTasks != 0) {
            _countCompleteTasksContainer.ActiveSelf();
            _countCompleteTasksText.text = _countCompleteTasks.ToString();
        }
        else {
            _countCompleteTasksContainer.DisactiveSelf();
        }
    }

    public void MinusOne() {
        _countCompleteTasks--;
        _countCompleteTasksText.text = _countCompleteTasks.ToString();
        if (_countCompleteTasks == 0) {
            _countCompleteTasksContainer.DisactiveSelf();
        }
        else if (_countCompleteTasks < 0) {
            Debug.Log($"Anomaly! Count complete task = {_countCompleteTasks} it`s < 0");
            _countCompleteTasks = 0;
        }
    }
}