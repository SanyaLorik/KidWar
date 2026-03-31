using Zenject;

public class AnimationsInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindShield();
        
    }

    private void BindShield() {
        Container.Bind<ShieldAnimationLikeBubble>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}