using System.Collections.Generic;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartBattleView : MonoBehaviour {
    [SerializeField] GameObject _container;
    
    [Header("Задний фон")]
    [SerializeField] Image _background;
    [SerializeField] PairedValue<float> _backgroundFadeTimes;
    [SerializeField] PairedValue<Ease> _backgroundEases;
    [Header("Приготовься")]
    [SerializeField] RectTransform _readyText;
    [SerializeField] PairedValue<float> _readyTextScaleTimes;
    [SerializeField] PairedValue<Ease> _getReadyEases;
    
    [Header("Вперёд!")]
    [SerializeField] RectTransform _goText;
    [SerializeField] RectTransform _preBattleContainer;
    [SerializeField] float _goTextScaleDuration;
    [SerializeField] Ease _goTextScaleEase;
    [SerializeField] float _conatinerGoBottomDuration;
    [SerializeField] Ease _goBottomScreenEase;

    [Header("Авы ботов!")]
    [SerializeField] private Image _leftAva;
    [SerializeField] private Image _rightAva;
    
    [Inject] List<SkinItemConfig> _skins;
    [Inject] ThrowGameStarter _throwGameStarter;
    [Inject] BattleManager _battleManager;

    
    private float _bootomScreenMoveDistance;
    private float _startContainerPose;

    public bool AnimationPlayNow { get; private set; }


    private void Start() {
        _container.DisactiveSelf();
        _startContainerPose = _preBattleContainer.anchoredPosition.y;
        _bootomScreenMoveDistance = _startContainerPose - _preBattleContainer.rect.height; 
    }

    private void InitAvatars(ObjectThrower left, ObjectThrower right) {
        _leftAva.sprite = _skins.Find(s => s.Id == left.SkinId).SkinSprite;
        _rightAva.sprite = _skins.Find(s => s.Id == right.SkinId).SkinSprite;
    }
    

    public void StartBattleAnimation(ObjectThrower leftPlayer, ObjectThrower rightPlayer) {
        if (_battleManager.MainPlayerPlay) {
            InitAvatars(leftPlayer, rightPlayer);
        }
        Debug.Log("StartBattleAnimation");
        SetAnimationPlayNow(true);
       
       // Появился черный экран
       Sequence sequence = DOTween.Sequence();
       sequence.Append(_background
           .DOFade(1f, _backgroundFadeTimes.From)
           .SetEase(_backgroundEases.From)
       );
       
       // Из нуля вылезает текст "Приготовься"
       sequence.Append(_readyText
           .DOScale(1f, _readyTextScaleTimes.From)
           .SetEase(_getReadyEases.From)
       );
       // С другой скоростью опять бежит в 0
       sequence.Append(_readyText
           .DOScale(0f, _readyTextScaleTimes.To)
           .SetEase(_getReadyEases.To)
       );
       // Исчезает бэкграунд
       sequence.Append(_background
           .DOFade(0f, _backgroundFadeTimes.To)
           .SetEase(_backgroundEases.From)
       );
       
       
       // Появляется Вперёд!
       sequence.Append(_goText
           .DOScale(1f, _goTextScaleDuration)
           .SetEase(_goTextScaleEase)
       );
       sequence.Append(_preBattleContainer
           .DOAnchorPosY(_bootomScreenMoveDistance, _conatinerGoBottomDuration)
           .SetEase(_goBottomScreenEase)
           .OnComplete(() => SetAnimationPlayNow(false))
       );
   }

    
    private void SetAnimationPlayNow(bool animationPlayNow) {
        Debug.Log("animationPlayNow = " + animationPlayNow);
        _container.SetActive(animationPlayNow);
        AnimationPlayNow = animationPlayNow;
        if (animationPlayNow) {
            // Подготовка
            Color tempColor = _background.color;
            tempColor.a = 0f;
            _background.color = tempColor;
            
            _preBattleContainer.anchoredPosition = new Vector2(_startContainerPose,_preBattleContainer.anchoredPosition.x);
            _readyText.localScale = Vector3.zero;
            _goText.localScale = Vector3.zero;
        }
    }
}
