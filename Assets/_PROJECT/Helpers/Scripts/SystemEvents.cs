using System;

public static class SystemEvents {
    public static event Action<bool> WindowOpened;
    public static event Action<bool> ForbidZoomChanged;
    public static event Action WindowScaleChanged;
    public static bool WindowIsOpen { get; private set; }
    
    
    public static void WindowOpen(bool state) {
        WindowOpened?.Invoke(state);
        WindowIsOpen = state;
    }
    
    public static void ForbidZoomChange(bool state) {
        ForbidZoomChanged?.Invoke(state);
    }

    public static void WindowScaleChange() {
        WindowScaleChanged?.Invoke();
    }

}