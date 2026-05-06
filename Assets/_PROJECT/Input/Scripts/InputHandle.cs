using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandle : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField] private float _minDistance = 10f;
    [SerializeField] private float _timer = 0.64f;

    public Vector2 Direction { get; private set; } = Vector2.zero;

    private CancellationTokenSource _tokenSource;

    private Vector2 _previous = Vector2.zero;
    private float _currentTimer = 0;

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _previous = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - _previous;
        _previous = eventData.position;

        float currentSpeed = delta.magnitude;
        print(delta + " " + currentSpeed);

        if (currentSpeed < _minDistance)
        {
            Direction = Vector2.zero;
            return;
        }

        _tokenSource?.Cancel();
        _tokenSource?.Dispose();

        _currentTimer = 0;

        _tokenSource = new();

        UniTask.Create(async () =>
        {
            while (_currentTimer < _timer)
            {
                _currentTimer += Time.deltaTime;

                await UniTask.Yield(cancellationToken: _tokenSource.Token);
            }

            Direction = Vector2.zero;
        });

        Direction = delta;
        Direction.Normalize();
    }
}