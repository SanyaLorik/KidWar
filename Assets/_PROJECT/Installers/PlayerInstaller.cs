using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private SkinItemConfig  _defaultSkinConfig;
    [SerializeField] private List<SkinItemConfig> _skinItemConfigs;

    
    
    public override void InstallBindings() {
        BindPlayerSingletones();
        BindPlayerSkin();
    }

    private void BindPlayerSingletones() {
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerStateManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void BindPlayerSkin() {
        Container.Bind<PlayerSkinWear>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<List<SkinItemConfig>>()
            .FromInstance(_skinItemConfigs)
            .AsSingle();
        
        // Skins
        Container.BindInterfacesAndSelfTo<PlayerSkinInventory>()
            .AsSingle()
            .WithArguments(_defaultSkinConfig)
            .NonLazy();
        
    }
}
