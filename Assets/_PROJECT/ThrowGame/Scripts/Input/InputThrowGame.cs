using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class InputThrowGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    [SerializeField] private float _requiredHoldTime = 0.2f; // Сколько нужно держать
    
    private CancellationTokenSource _tokenSource;
    private float _chargeTime = 0f;
    private bool _isDownInvoked;
    
    public event Action OnUpped;
    public event Action OnDowned;
    public event Action<Vector2> OnDragged;

    public Vector2 DragDelta { get; private set; } = Vector2.zero;

    public void OnPointerUp(PointerEventData eventData)
    {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        if (_chargeTime >= _requiredHoldTime) {
            OnUpped?.Invoke();
        }
        _chargeTime = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Нажал хуем на экран
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        _chargeTime = 0f;
        ChargeTimerAsync(_tokenSource.Token).Forget();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        DragDelta = eventData.delta;
        OnDragged?.Invoke(DragDelta);
    }

    private async UniTask ChargeTimerAsync(CancellationToken token) 
    {
        _isDownInvoked = false;
        while (!token.IsCancellationRequested) {
            _chargeTime += Time.deltaTime;
            if (_chargeTime >= _requiredHoldTime && !_isDownInvoked) {
                OnDowned?.Invoke();
                _isDownInvoked = true;
            }
            await UniTask.Yield();
        }
    }

}