using System;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class Narrator : MonoBehaviour {
    [SerializeField] private GameObject _narratorContainer;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Transform _screenFinger;
    [SerializeField] private Transform _healFinger;
    [SerializeField] private Transform _explosionFinger;

    [Header("Анимация пальчика")]
    [SerializeField] private float _fingerDuration;
    [SerializeField] private Ease _fingerEase;
    
    [Header("Высота подьема пальчика")]
    [SerializeField] private float _screenFingerDeltaY;
    [SerializeField] private float _healFingerDeltaY;
    [SerializeField] private float _explosionFingerDeltaY;
    
    
    private Transform _currentFinger;
    
    [Inject] private LocalizationData _localization;
    [Inject] private TutorialManager _tutorialManager;
    

    private void OnEnable() {
        if (_tutorialManager.TutorialPassed) {
            _narratorContainer.DisactiveSelf();
        }
        else {
            _tutorialManager.NewTutorialStep += CheckToDestroyCurrentFinger;
        }
    }

    
    public void Disactive() {
        _narratorContainer.DisactiveSelf();
        _screenFinger.DisactiveSelf();
        _healFinger.DisactiveSelf();
        _explosionFinger.DisactiveSelf();
    }

    
    public void DisableNarrator() {
        Disactive();
        _tutorialManager.NewTutorialStep -= CheckToDestroyCurrentFinger;
    }

    
    public void Active() {
        _narratorContainer.ActiveSelf();
    }
    
    
    public void CheckToDestroyCurrentFinger() {
        if (_currentFinger != null && _currentFinger.gameObject.activeSelf) {
            _currentFinger.DisactiveSelf();
        }
    }
    

    public void SetTutorialText(TutorialStep step) {
        _narratorContainer.ActiveSelf();
        _text.text = _localization.GetTranslatedText(step, _localization.TutorialTranslates);
    }


    public void ShowScreenFinger() {
        _screenFinger.ActiveSelf();
        AnimateFinger(_screenFinger, _screenFingerDeltaY);
    }
    
    
    public void ShowHealFinger() {
        _healFinger.ActiveSelf();
        AnimateFinger(_healFinger, _healFingerDeltaY);
    }

    
    public void ShowExplosionFinger() {
        _explosionFinger.ActiveSelf();
        AnimateFinger(_explosionFinger, _explosionFingerDeltaY);
    }

    
    private void AnimateFinger(Transform finger, float _fingerDelta) {
        _currentFinger = finger;
        finger
            .DOMoveY(finger.position.y + _fingerDelta, _fingerDuration)
            .SetEase(_fingerEase)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(finger.gameObject);
    }

}