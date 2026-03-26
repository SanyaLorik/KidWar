using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class InputThrowGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public event Action OnUpped = delegate { };
    public event Action OnDowned = delegate { };
    public event Action<Vector2, Vector2> OnDragged = delegate { };

    public Vector2 DragPosition { get; private set; } = Vector2.zero;

    public Vector2 DragDelta { get; private set; } = Vector2.zero;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUpped.Invoke();

        DragPosition = Vector2.zero;
        DragDelta = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnUpped.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragPosition = eventData.position;
        DragDelta = eventData.delta;

        OnDragged.Invoke(eventData.position, eventData.delta);
    }
}