using UnityEngine;
using Zenject;

public class BotInstaller: MonoInstaller {
    [SerializeField] private Transform[] _pointsToWalk;
    
    public override void InstallBindings() {
        BindBotStateManager();
        BindWalkPoints();
        BindPlayerCopyBot();
    }

    private void BindBotStateManager() {
        
        Container.BindInterfacesAndSelfTo<BotsMainManager>().AsSingle().NonLazy();
        Container.Bind<BotStateManager>()
            .FromComponentsInHierarchy()
            .AsTransient();

    }

    private void BindWalkPoints() {
        Container.Bind<Transform[]>()
            .WithId("WalkPoints")
            .FromInstance(_pointsToWalk)
            .AsSingle().NonLazy();
    }

    private void BindPlayerCopyBot() {
        Container.BindInterfacesAndSelfTo<PlayerCopyBotController>()
            .FromComponentsInHierarchy()
            .AsSingle()
            .NonLazy();
    }

}
