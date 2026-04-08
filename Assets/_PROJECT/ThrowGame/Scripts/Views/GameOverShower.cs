using System;
using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverShower : MonoBehaviour {
    [SerializeField] private GameObject _allContainer;
    [SerializeField] private GameObject _playgroundContainer;
    
    // PVP
    [Header("PVB")]
    [SerializeField] private GameObject _pvbContainer;
    [SerializeField] private GameObject _winContainer;
    [SerializeField] private GameObject _loseContainer;
    [Header("PVP")]
    [SerializeField] private GameObject _pvpContainer;

    
    
    [SerializeField] private TextMeshProUGUI _winnerNumberText;
    
    
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _continue2xButton;

    public bool ResultWindowShowing { get; private set; }
    public event Action<bool> PlayerWon;

    [Inject] private ThrowGameStarter _gameStarter;
    [Inject] private BattleManager _battleManager;
    [Inject] private PlayerSkinInventory _skinInventory;
    [Inject] private BotsMainManager _botsMainManager;
    [Inject] private IGameSave _gameSave;
    [Inject] private CameraOrbitalController _camera;

    private void Start() {
        _allContainer.DisactiveSelf();
    }

    private void OnEnable() {
        _continueButton.onClick.AddListener(CloseResultWindow);
        _continue2xButton.onClick.AddListener(CloseResultWindow);
    }

    public void HidePlayCanvas() {
        _playgroundContainer.DisactiveSelf();
    } 
    
    public void ShowResults() {
        ResultWindowShowing = true;
        MoveCameraToWinner();
        HidePlayCanvas();
        _allContainer.ActiveSelf();
        // PVP
        if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
            SetResultState(MainPlayerWinner() ? 1 : 2);
            return;
        }
        // PVB
        SetPvbState(MainPlayerWinner());
    }

    
    private void SetPvbState(bool mainPlayerWin) {
        _pvbContainer.ActiveSelf();
        _pvpContainer.DisactiveSelf();
        
        _winContainer.SetActive(mainPlayerWin);
        _loseContainer.SetActive(!mainPlayerWin);
        
        PlayerWon?.Invoke(mainPlayerWin);
    }
    
    private void SetResultState(int winnerNumber) {
        _pvbContainer.DisactiveSelf();
        _pvpContainer.ActiveSelf();
        
        _winnerNumberText.text = winnerNumber.ToString();
    }
    
    private void MoveCameraToWinner() {
        if (MainPlayerWinner()) {
            _camera.GoToWinner(_battleManager.FirstThrower.ObjectThrower.PointToBeat);
            _camera.SetLeftPlayerWinnerAxises(true);
        }
        else {
            _camera.GoToWinner(_battleManager.SecondThrower.ObjectThrower.PointToBeat);
            _camera.SetLeftPlayerWinnerAxises(false);
        }
    }

    private void CloseResultWindow() {
        _playgroundContainer.ActiveSelf();
        ResultWindowShowing = false;
        _allContainer.DisactiveSelf();
    }

    // Типо ход остановился на игроке
    private bool MainPlayerWinner()
        => _battleManager.IsFirstThrowerStep;  

        

}
