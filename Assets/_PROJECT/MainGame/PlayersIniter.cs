using UnityEngine;
using Zenject;


// Настройка положения игроков во время игры, поворот и тп
public class PlayersIniter : MonoBehaviour {
    [field: Header("Левая правая точки игроков")]
    [field: SerializeField] public Transform LeftPoint { get; private set; }
    [field: SerializeField] public Transform RightPoint { get; private set; }
    [field: Header("Отступ от игрока при макс броске")]
    [field: SerializeField] public float OffsetToMaxThrow { get; private set; }
    
    
    [Inject] PlayerMovement _playerMovement;
    
    public void InitForNewGame() {
        Transform playerPoint = Random.value < 0.5f ? LeftPoint : RightPoint;
        Transform secondPlayerPoint = playerPoint == LeftPoint ? RightPoint : LeftPoint;
        _playerMovement.TpPlayerInPoint(playerPoint, 3f);
        RotateToTarget(_playerMovement, secondPlayerPoint.position);
    }


    
    private void RotateToTarget(PlayerMovement player, Vector3 targetPosition) {
        Vector3 direction = targetPosition - player.transform.position;
        direction.y = 0;
        player.SetCharacterControllerState(false);
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = targetRotation;
        }
        player.SetCharacterControllerState(true);
    }
    
}