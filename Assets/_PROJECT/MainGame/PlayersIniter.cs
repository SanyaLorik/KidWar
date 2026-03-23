using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
public class PlayersIniter : MonoBehaviour {
    [field: Header("Левая правая точки игроков")]
    [field: SerializeField] public Transform LeftPoint { get; private set; }
    [field: SerializeField] public Transform RightPoint { get; private set; }
    [field: Header("Отступ от игрока при макс броске")]
    [field: SerializeField] public float OffsetToMaxThrow { get; private set; }


    [SerializeField] private ObjectThrower _mainPlayer;
    
    [Inject] PlayerMovement _playerMovement;
    [Inject] BotsMainManager _botsMainManager;
    
    
    
    /// <summary>
    /// Поидее надо будет указывать режим PVP PVB
    /// </summary>
    public void InitForNewGame() {
        // Настройка первого игрока
        Transform playerPoint = LeftPoint;
        _playerMovement.TpPlayerInPoint(playerPoint);
        _playerMovement.RotateToTarget(RightPoint.position);
        
        // Настройка второго игрока
        Transform secondPlayerPoint = RightPoint;
        BotStateManager secondPlayer = _botsMainManager.GetRandomBot();
        secondPlayer.TpInPoint(secondPlayerPoint.position);
        secondPlayer.RotateToTarget(LeftPoint.position);

    }

    
    
}