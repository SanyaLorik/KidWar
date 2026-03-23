using Zenject;

public class ThrowInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindThrow();
    }
    
    
    private void BindThrow() {
        Container.Bind<ObjectThrower>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ThrowGameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<Trajectory3D>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayersIniter>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}