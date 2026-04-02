using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class BattleInformator : MonoBehaviour {
    [SerializeField] private GameObject _textFieldContainer;
    [SerializeField] private TextMeshProUGUI _textFieldToInformate;
    [SerializeField] private TextMeshProUGUI _secondPlayerNickname;
    [SerializeField] private float _timeToShowInfo;

    private CancellationTokenSource _tokenSource;
    
    
    [Inject] private BattleManager _battleManager;
    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private LocalizationData _localization;
    [Inject] private HpView _hpView;
    
    
    private void OnEnable() {
        _gameStarter.GameStarted += OnGameStarted;
        _hpView.PlayerHit += HpViewOnPlayerHit;
    }

    private void OnGameStarted(bool started) {
        // Игра началась выводим имена
        if (started) {
            if (_battleManager.MainPlayerPlay) {
                // По сути просто если там бот то подставляем его ник, если игра 1 на 1 то оставляем просто "Враг"
                if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
                    _secondPlayerNickname.text = _localization.Enemy;
                }
                else {
                    _secondPlayerNickname.text = _battleManager.SecondThrower.ObjectThrower.Nickname;
                }
            }
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

    public string GetWinnerName() {
        string winnerName = 
            _battleManager.IsFirstThrowerStep  
            ?
            _battleManager.FirstThrower.ObjectThrower.Nickname
            :
            _battleManager.SecondThrower.ObjectThrower.Nickname;
        return winnerName;
    }


    private void HpViewOnPlayerHit() {
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
