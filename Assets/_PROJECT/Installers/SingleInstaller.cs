using UnityEngine;
using Zenject;

public class SingleInstaller : MonoInstaller {
    [SerializeField] private GameObject _canvasToHide;
    
    public override void InstallBindings() {
        BindCamera();
        BindSettings();
        BindValuteFormatter();
        BindNicknameRandomizer();
        Container.Bind<AdvTimerStarter>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TasksManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        BindTurorial();
        BindCanvasToHide();
    }

    private void BindTurorial() {
        Container.Bind<TutorialManager>().FromComponentInHierarchy().AsSingle().NonLazy();
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
        Container.BindInterfacesAndSelfTo<NicknameRandomizer>().AsSingle().NonLazy();
    }
    
    
    private void BindCanvasToHide() {
        Container.Bind<GameObject>()
            .WithId("CanvasToHide")
            .FromInstance(_canvasToHide)
            .AsSingle().NonLazy();
    }
    

}