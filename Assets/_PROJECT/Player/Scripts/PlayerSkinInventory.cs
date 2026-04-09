using System;
using System.Linq;
using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerSkinInventory : IInitializable {
    public event Action<SkinItemConfig> SkinUnlocked;
    public event Action<SkinItemConfig> SkinEquipped;
    public readonly SkinItemConfig DefaultSkinConfig; 
    private GameSave Saves => _saver.GetSave<GameSave>();
    
    [Inject] private IGameSave _saver; 

    public PlayerSkinInventory(SkinItemConfig defaultSkinConfig) {
        DefaultSkinConfig = defaultSkinConfig;
    }
    
    public void Initialize() {
        // Если никого скина нет вообще
        if (!SkinIsBought(DefaultSkinConfig.Id)) {
            Saves.AddNewSkin(DefaultSkinConfig.Id);
            EquipSkin(DefaultSkinConfig);
        }
    }
    
    public bool SkinIsBought(string id) 
        => Saves.Skins.Any(s => s.Id == id);


    public string GetRandomPlayerBoughtSkinId()
        => Saves.Skins.GetRandomElement().Id;
    
    
    public void UnlockSkin(SkinItemConfig skinItemConfig) {
        Saves.AddNewSkin(skinItemConfig.Id);
        _saver.Save();
        SkinUnlocked?.Invoke(skinItemConfig);
    }
    
    
    public void EquipSkin(SkinItemConfig skinItemConfig) {
        Saves.SkinWearId = skinItemConfig.Id;
        _saver.Save();
        SkinEquipped?.Invoke(skinItemConfig);
        Debug.Log("SkinEquipped");
    }
    
    
    public string CurrentSkinId => Saves.SkinWearId;
    
}
