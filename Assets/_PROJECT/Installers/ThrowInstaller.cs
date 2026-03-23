using Zenject;

public class ThrowInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindThrow();
    }
    
    
    private void BindThrow() {
        Container.Bind<ThrowGameStarter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayersIniter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ObjectThrowerCalculator>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}