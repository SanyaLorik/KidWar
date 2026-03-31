using Architecture_M;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSave : GameSaveBase {
    public long Money;
    public bool IsBoughtPurchase = false;
    
    // Skins
    public List<SkinItem> Skins = new ();
    public string SkinWearId = "";
    
    // Bonuses
    public List<BonuseItem> Bonuses = new ();
    
    
    public void AddNewSkin(string id) {
        if(Skins.Any(s => s.Id == id)) return;
        Skins.Add(new SkinItem {
            Id = id,
        });
    }
    
    
    public void AddNewBonusCounts(string id, int count) {
        BonuseItem bonus = Bonuses.FirstOrDefault(b => b.Id == id);
        if (bonus == null) {
            Bonuses.Add(new BonuseItem {
                Id = id,
                Count = count,
            });
            return;
        }
        bonus.Count += count;
    }

    public int GetBonusCount(string id) {
        BonuseItem bonus = Bonuses.FirstOrDefault(b => b.Id == id);
        var count = bonus == null ? 0 : bonus.Count;
        Debug.Log($"Id бонуса игрока: {id}, количество: {count}");
        return count;
    }
    
    public void SetMinusOneBonus(string id) {
        BonuseItem bonus = Bonuses.FirstOrDefault(b => b.Id == id);
        if (bonus!=null) {
            --bonus.Count;
            Debug.Log($"Минус 1 бонус {id}, всего их {bonus.Count}");
        }
        else {
            Debug.LogError("У игрока нет такого бонуса, ошибка в коде");
        }
    }
}

[Serializable]
public class SkinItem {
    public string Id = "";
}


[Serializable]
public class BonuseItem {
    public string Id = "";
    public int Count = 0;
}