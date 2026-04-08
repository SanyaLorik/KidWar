using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BattleInformator : MonoBehaviour {
    [SerializeField] private GameObject _textFieldContainer;
    [SerializeField] private TextMeshProUGUI _textFieldToInformate;
    [SerializeField] private TextMeshProUGUI _secondPlayerNickname;
    [SerializeField] private float _timeToShowInfo;

    [Header("Авы ботов!")]
    [SerializeField] private Image _leftAva;
    [SerializeField] private Image _rightAva;
    
    private CancellationTokenSource _tokenSource;
    
    [Inject] List<SkinItemConfig> _skins;
    [Inject] private BattleManager _battleManager;
    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private LocalizationData _localization;
    [Inject] private HpView _hpView;
    
    
    private void OnEnable() {
        _gameStarter.GameStarted += OnGameStarted;
        _hpView.PlayerHit += OnPlayerHit;
    }

    private void OnGameStarted(bool started) {
        // Игра началась выводим имена
        if (started && _battleManager.MainPlayerPlay) {
            ShowNicknames();
            InitAvatars();
        }
        // Игра кончилась епта выводим инфу о победителе по кол-ву хп видимо...
        else {
            if (_battleManager.MainPlayerPlay) {
                // Показ окна побёды
                Debug.Log("Показ окна победы");
            }
            else {
                string info = string.Format(_localization.PlayerWinner, GetWinnerName());
                ShowInfo(info);
            }
        }
    }
    
    private void InitAvatars() {
        ObjectThrower left = _battleManager.FirstThrower.ObjectThrower;
        ObjectThrower right = _battleManager.SecondThrower.ObjectThrower;
        _leftAva.sprite = _skins.Find(s => s.Id == left.SkinId).SkinSprite;
        _rightAva.sprite = _skins.Find(s => s.Id == right.SkinId).SkinSprite;
    }

    private void ShowNicknames() {
       
            // По сути просто если там бот то подставляем его ник, если игра 1 на 1 то оставляем просто "Враг"
            if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
                _secondPlayerNickname.text = _localization.Enemy;
            }
            else {
                _secondPlayerNickname.text = _battleManager.SecondThrower.ObjectThrower.Nickname;
            }
    }

    public string GetWinnerName() {
        string winnerName = 
            _battleManager.IsFirstThrowerStep  
            ?
            _battleManager.FirstThrower.ObjectThrower.Nickname
            :
            _battleManager.SecondThrower.ObjectThrower.Nickname;
        return winnerName;
    }


    private void OnPlayerHit() {
        if (_battleManager.MainPlayerPlay) return;

        string playerName = _battleManager.IsFirstThrowerStep
            ? _battleManager.FirstThrower.ObjectThrower.Nickname
            : _battleManager.SecondThrower.ObjectThrower.Nickname;
        
        string info = string.Format(
            _localization.PlayerHit,
            playerName
        );
        ShowInfo(info);
    }

    private void ShowInfo(string info) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ShowInfoAsync(info, _tokenSource.Token).Forget();
    }

    private async UniTask ShowInfoAsync(string text, CancellationToken token) {
        _textFieldContainer.ActiveSelf();
        _textFieldToInformate.text = text;
        await UniTask.WaitForSeconds(_timeToShowInfo, cancellationToken: token);
        _textFieldContainer.DisactiveSelf();
    }
}
