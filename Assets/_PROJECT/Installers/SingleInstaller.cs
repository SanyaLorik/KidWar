using Zenject;

public class SingleInstaller : MonoInstaller {
    public override void InstallBindings() {
        BindCamera();
        BindSettings();
        BindValuteFormatter();
        BindNicknameRandomizer();
    }

    private void BindCamera() {
        Container.Bind<CameraOrbitalController>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void BindSettings() {
        Container.Bind<SettingsManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    } 
    
    private void BindValuteFormatter() {
        Container.Bind<NumberFormatter>().AsSingle().NonLazy();
    }
    
    private void BindNicknameRandomizer() {
        Container.Bind<NicknameRandomizer>().AsSingle().NonLazy();
    }
    

}