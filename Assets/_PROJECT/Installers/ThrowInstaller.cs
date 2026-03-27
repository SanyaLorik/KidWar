using Zenject;

public class ThrowInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindThrow();
    }
    
    
    private void BindThrow() {
        Container.Bind<ThrowGameStarter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BattleManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ObjectThrowerCalculator>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<HpView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<WindChooseView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TimerToThrowStep>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ForceChooseView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<InputThrowGame>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ModifierManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BonusManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}