using System;

public static class GameEvents {
    public static event Action PlayerHited;
    public static event Action FloorHited;
    public static event Action ObjectExploded;
    public static event Action ObjectGianted;
    public static event Action ModifierReloaded;

    private static BattleManager _battleManager;

    public static void FloorInvoke() {
        FloorHited?.Invoke();
    }
    
    public static void PlayerHitInvoke() {
        PlayerHited?.Invoke();
    }
    
    public static void ObjectExplodeInvoke() {
        ObjectExploded?.Invoke();
    }
    
    public static void GiantModifierInvoke() {
        ObjectGianted?.Invoke();
    }
    

    public static void ModifierReloadedInvoke() {
        ModifierReloaded?.Invoke();
    }
    
}
