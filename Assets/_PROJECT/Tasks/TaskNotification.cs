using System.Collections;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class TaskNotification : MonoBehaviour {
    // [SerializeField] private RectTransform _panel;
    // [SerializeField] private RectTransform _canvas;
    // [SerializeField] private RectTransform _screenPosition;
    // [SerializeField] private RectTransform _behindScreenPosition;
    // [SerializeField] private float _timeToShow;
    // [SerializeField] private float _duration;
    //
    // // TEXT
    // [SerializeField] private TMP_Text _moneyText;
    // [SerializeField] private TMP_Text _taskCompletedText;
    // [SerializeField] private TMP_Text _getRewardTextTaskNotification;
    // private bool _notifIsShowed;
    //
    //
    // [Inject] private LocalizationData _localization;
    //
    //
    // private void Awake() {
    //     _canvas.DisactiveSelf();
    //     _panel.anchoredPosition = _behindScreenPosition.anchoredPosition; // сразу прячем
    // }
    //
    // private void Start() {
    //     _taskCompletedText.text = _localization.TaskCompletedNotification;
    //     _getRewardTextTaskNotification.text = _localization.CollectRewardTaskNotification;
    // }
    //
    //
    // private Coroutine _notifCoroutine;
    // public void ShowNotification(string money) {
    //     _moneyText.text = money;
    //     if (_notifCoroutine != null) {
    //         StopCoroutine(_notifCoroutine);
    //     }
    //     _notifCoroutine = StartCoroutine(NotificationRoutine());
    // }
    //
    // private IEnumerator NotificationRoutine() {
    //     Show();
    //     yield return new WaitUntil(() => _notifIsShowed);
    //     yield return new WaitForSeconds(_timeToShow);
    //     Hide();
    //     _notifCoroutine = null;
    //     
    // }
    //     
    // private void Show() {
    //     _canvas.ActiveSelf();
    //     _panel.DOAnchorPos(_screenPosition.anchoredPosition, _duration)
    //         .SetEase(Ease.OutBack)
    //         .OnComplete(() => _notifIsShowed = true);
    // }
    //
    // private void Hide() {
    //     _panel.DOAnchorPos(_behindScreenPosition.anchoredPosition, _duration)
    //         .SetEase(Ease.InBack)
    //         .OnComplete(_canvas.DisactiveSelf);
    //     _notifIsShowed = false;
    // }
    
    
    
    
    
}
