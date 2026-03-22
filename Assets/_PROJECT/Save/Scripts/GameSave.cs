using Architecture_M;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameSave : GameSaveBase {
    public long Money;
    public bool IsBoughtPurchase = false;
    
    
    // Skins
    public List<Skin> Skins = new ();
    public string SkinWearId = "";
    
    public void AddNewSkin(string id) {
        if(Skins.Any(s => s.Id == id)) return;
        Skins.Add(new Skin {
            Id = id,
        });
    }
    
}

[Serializable]
public class Skin {
    public string Id = "";
}