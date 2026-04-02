using System;
using System.Linq;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PlayerSkinInventory : IInitializable {
    public event Action<SkinItemConfig> SkinUnlocked;
    public event Action<SkinItemConfig> SkinEquipped;
    private readonly SkinItemConfig _defaultSkinConfig; 
    
    [Inject] private IGameSave _saver; 

    public PlayerSkinInventory(SkinItemConfig defaultSkinConfig) {
        _defaultSkinConfig = defaultSkinConfig;
    }
    
    public void Initialize() {
        // Если никого скина нет вообще
        if (!SkinIsBought(_defaultSkinConfig.Id)) {
            _saver.GetSave<GameSave>().AddNewSkin(_defaultSkinConfig.Id);
            EquipSkin(_defaultSkinConfig);
        }
    }
    
    public bool SkinIsBought(string id) 
        => _saver.GetSave<GameSave>().Skins.Any(s => s.Id == id);


    public string GetRandomPlayerBoughtSkinId()
        => _saver.GetSave<GameSave>().Skins.GetRandomElement().Id;
    
    
    public void UnlockSkin(SkinItemConfig skinItemConfig) {
        _saver.GetSave<GameSave>().AddNewSkin(skinItemConfig.Id);
        _saver.Save();
        SkinUnlocked?.Invoke(skinItemConfig);
    }
    
    
    public void EquipSkin(SkinItemConfig skinItemConfig) {
        _saver.GetSave<GameSave>().SkinWearId = skinItemConfig.Id;
        _saver.Save();
        SkinEquipped?.Invoke(skinItemConfig);
    }
    
    
    public string CurrentSkinId => _saver.GetSave<GameSave>().SkinWearId;
    
}
