using System.Collections;
using UnityEngine;

public class WindowChangedNotifier: MonoBehaviour {
    private int _lastWidth;
    private int _lastHeight;
    private bool _isQuitting = false;
    private Coroutine _pendingCoroutine;
    
    
    private void Awake() {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }
    
    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void OnDestroy()
    {
        _isQuitting = true;
    }

    private void OnRectTransformDimensionsChange() {
        if (_isQuitting || this == null || gameObject == null)
            return;
        
        // Защита от ложных вызовов
        if (Screen.width != _lastWidth || Screen.height != _lastHeight) {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            
            
            // Останавливаем предыдущую корутину, если была
            if (_pendingCoroutine != null)
                StopCoroutine(_pendingCoroutine);
                
            // Запускаем новую
            _pendingCoroutine = StartCoroutine(WaitAndNotify());
        }
    }
    
    private IEnumerator WaitAndNotify()
    {
        yield return new WaitForSeconds(.5f);
        
        // Проверяем, живы ли мы еще
        if (this != null && gameObject != null) 
        {
            Debug.Log($"Реальное изменение размера окна: {_lastWidth} x {_lastHeight}");
            SystemEvents.WindowScaleChange();
            _pendingCoroutine = null;
        }
    }
}