using Architecture_M;
using LuringPlayer_M;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSave : GameSaveBase,
    IDailyRewardSaveLoader, IWheelFortuneSaveLoader
{
    public long Money;
    public bool IsBoughtPurchase = false;
    public bool TutorialPassed = false;
    
    // Skins
    public List<SkinItem> Skins = new ();
    public string SkinWearId = "";
    
    // Bonuses
    public List<BonuseItem> Bonuses = new ();
    
    // Task
    public List<TaskItem> Tasks = new ();

    
    // Daily Rewards
    public DailyRewardSave DailyRewardSave;
    public WheelFortuneSave WheelFortuneSave;

    public void AddNewSkin(string id) {
        if(Skins.Any(s => s.Id == id)) return;
        Skins.Add(new SkinItem {
            Id = id,
        });
    }
    
    
    public void AddNewBonusCounts(string id, int count, bool clear = false) {
        BonuseItem bonus = Bonuses.FirstOrDefault(b => b.Id == id);
        if (bonus == null) {
            Bonuses.Add(new BonuseItem {
                Id = id,
                Count = count,
            });
        }
        else {
            if (clear) {
                bonus.Count = count;
            }
            else {
                bonus.Count += count;
            }
            Debug.Log($"Added {count} bonus: {bonus.GetType()}"); 
        }
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
            // Debug.Log($"Минус 1 бонус {id}, всего их {bonus.Count}");
        }
        else {
            // Debug.LogError("У игрока нет такого бонуса, ошибка в коде");
        }
    }

    // Tasks--------------------------------
    public void UpdateTaskInfo(string id, int count, bool isGetReward) {
        TaskItem task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task != null) {
            task.Count = count;
            task.IsGetReward = isGetReward;
        }
        else {
            Debug.LogError("Задача не была инициализирована");
        }
    }
    
    /// <summary>
    /// Используется для получения таски, если игрок играет впервые то инициализирует оную
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TaskItem GetTaskInfo(string id) {
        TaskItem task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) {
            task = new TaskItem() {
                Id = id,
                Count = 0,
                IsGetReward = false
            };
            Tasks.Add(task);
        }
        return task;
    }

    public DailyRewardSave Load()
    {
        return DailyRewardSave;
    }

    WheelFortuneSave IWheelFortuneSaveLoader.Load()
    {
        return WheelFortuneSave;
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

[Serializable]
public class TaskItem {
    public string Id = "";
    public int Count = 0;
    public bool IsGetReward = false;
}