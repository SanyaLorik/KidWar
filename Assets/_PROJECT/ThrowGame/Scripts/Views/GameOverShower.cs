using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverShower : MonoBehaviour {
    [SerializeField] private GameObject _allContainer;
    [SerializeField] private GameObject _winContainer;
    [SerializeField] private GameObject _loseContainer;
    [SerializeField] private GameObject _resultContainer;
    
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _continue2xButton;

    public bool ResultWindowShowing { get; private set; }

    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private BattleManager _battleManager;
    [Inject] private PlayerSkinInventory _skinInventory;
    [Inject] private BotsMainManager _botsMainManager;
    [Inject] private IGameSave _gameSave;


    private void OnEnable() {
        _continueButton.onClick.AddListener(CloseResultWindow);
        _continue2xButton.onClick.AddListener(CloseResultWindow);
    }


    public void ShowResults() {
        ResultWindowShowing = true;
        // PVP
        if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
            SetPvpHeader();
            return;
        }
        // PVB
        if (MainPlayerWinner()) {
            SetWinHeader();
        }
        else {
            SetLoseHeader();
        }
        
    }
    
    private void CloseResultWindow() {
        ResultWindowShowing = false;
        _allContainer.DisactiveSelf();
    }

    // Типо ход остановился на игроке
    private bool MainPlayerWinner()
        => _battleManager.IsFirstThrowerStep;  
                


    private void SetWinHeader() {
        _allContainer.ActiveSelf();
        _winContainer.ActiveSelf();
        _loseContainer.DisactiveSelf();
        _resultContainer.DisactiveSelf();
    }
        
    private void SetLoseHeader() {
        _allContainer.ActiveSelf();
        _winContainer.DisactiveSelf();
        _loseContainer.ActiveSelf();
        _resultContainer.DisactiveSelf();
    }
    
    private void SetPvpHeader() {
        _allContainer.ActiveSelf();
        _winContainer.DisactiveSelf();
        _loseContainer.DisactiveSelf();
        _resultContainer.ActiveSelf();
    }

}
