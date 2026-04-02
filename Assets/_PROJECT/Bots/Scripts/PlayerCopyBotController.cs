using UnityEngine;
using Zenject;

public class PlayerCopyBotController : MonoBehaviour {
    [SerializeField] private BotStateManager _hiddenBot;

    [Inject] private ThrowGameStarter _gameStarter ;
    [Inject] private BattleManager _battleManager;
    [Inject] private PlayerSkinInventory _skinInventory;
    [Inject] private BotsMainManager _botsMainManager;
    
    
    
    private void Awake() {
        _hiddenBot.SetPlayerCopyBehavior();
        _hiddenBot.EnableBot(false);
    }

    public BotStateManager GetBotToBattle() {
        _hiddenBot.EnableBot(true);
        string randomPlayerBoughtSkinId = _skinInventory.GetRandomPlayerBoughtSkinId();
        _botsMainManager.SetBotSkin(_hiddenBot, randomPlayerBoughtSkinId);
        return _hiddenBot;
    }
    
    private void OnEnable() {
        _gameStarter.GameStarted += OnGameStarted;
    }

    private void OnGameStarted(bool started) {
        if (!started) {
            if (_battleManager.SecondThrower.ObjectThrower.PlayerHandle) {
                _hiddenBot.EnableBot(false);
            }
        }
    }

}
