using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PlayersStepView : MonoBehaviour {
    [Header("Большой")]
    [SerializeField] private Transform _playerStepBigContainer;
    [SerializeField] private Transform _enemyStepBigContainer;
    [Header("Настройка времени и анимаций")]
    [SerializeField] private PairedValue<Ease> _showBigEases;

    [field: SerializeField] public float TimeToShowBig { get; private set; } = .7f;
    [field:  SerializeField] public float TimeToHideBig { get; private set; } = .7f;
    [SerializeField] private Ease _hideEase;
    [Header("Маленький")]
    [SerializeField] private RectTransform _yourStepSmall;
    [SerializeField] private RectTransform _enemyStepSmall;
    [SerializeField] private Transform _targetToContiner;
    
    public bool AnimationIsShowing { get; private set; }

    private Vector3 _startPos;
    private bool _firstTimeShow = true;
    
    [Inject] TutorialManager _tutorialManager;
    

    private void Start() {
        _startPos = _playerStepBigContainer.position;
        _playerStepBigContainer.gameObject.DisactiveSelf();
        _enemyStepSmall.gameObject.DisactiveSelf();
        _enemyStepBigContainer.gameObject.DisactiveSelf();
        _yourStepSmall.gameObject.DisactiveSelf();
    }

    public void ShowPlayerStep(bool isFirstThrowerStep) {
        if (!_tutorialManager.TutorialPassed) {
            TutorialAnimate(isFirstThrowerStep);
            return;
        }
        
        // Leftning
        if (isFirstThrowerStep) {
            // Pulse
            _yourStepSmall.gameObject.ActiveSelf();
            _enemyStepSmall.gameObject.DisactiveSelf();
            AnimateBigAttention(_playerStepBigContainer);
        }
        else {
            // Pulse
            _enemyStepSmall.gameObject.ActiveSelf();
            _yourStepSmall.gameObject.DisactiveSelf();
            AnimateBigAttention(_enemyStepBigContainer);
        }
    }

    
    private void TutorialAnimate(bool isFirstThrowerStep) {
        if (_firstTimeShow) {
            _firstTimeShow = false;
            _yourStepSmall.gameObject.DisactiveSelf();
            _enemyStepSmall.gameObject.DisactiveSelf();
            return;
        }
        AnimateBigAttention(isFirstThrowerStep ? _playerStepBigContainer : _enemyStepBigContainer);
    }
    

    private void AnimateBigAttention(Transform rectTransformContainer) {
        AnimationIsShowing = true;
        // Вернуть обратно
        rectTransformContainer.transform.position = _startPos;
        rectTransformContainer.gameObject.ActiveSelf();
        rectTransformContainer.localScale = Vector3.zero;
        
        Sequence sequence = DOTween.Sequence();
        // Появление
        sequence.Append(rectTransformContainer
            .DOScale(1, TimeToShowBig)
            .SetEase(_showBigEases.From)
        );
        // sequence.Join(rectTransformContainer
        //     .DOMove(_startPos, TimeToShowBig)
        //     .SetEase(_showBigEases.From)
        // );
        // Уменьшение
        sequence.Append(rectTransformContainer
            .DOScale(0f, TimeToHideBig)
            .SetEase(_showBigEases.To)
        );
        // Вниз убегатывание
        
        // Debug.Log("_targetToContiner.transform.position = " + _targetToContiner.transform.position);
        
        sequence.Join(rectTransformContainer
            .DOMove(_targetToContiner.transform.position, TimeToHideBig)
            .SetEase(_hideEase)
            .OnComplete(() => AnimationIsShowing = false)
        );

    }

    
}
