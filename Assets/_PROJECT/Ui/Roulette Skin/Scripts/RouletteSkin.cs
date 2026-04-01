using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;

public class RouletteSkin : MonoBehaviour
{
    [SerializeField] private int _spinCount;
    [SerializeField] private float _duration;
    [SerializeField] private AnimationCurve _behaviourCurve;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform[] _items;

    private void Start()
    {
        SpinAsync().Forget();
    }

    private void ShiftArray()
    {
        var last = _items[^1];
        for (int i = _items.Length - 1; i >= 1; i--)
            _items[i] = _items[i - 1];

        _items[0] = last;
    }

    private async UniTaskVoid SpinAsync()
    {
        float expendedTime = 0;

        float spacing = _items[1].anchoredPosition.x - _items[0].anchoredPosition.x;
        float borderX = _rect.anchoredPosition.x + _rect.rect.width;
        float replaceX = borderX + spacing;

        float distance = (_items[^1].anchoredPosition.x + spacing) - _items[0].anchoredPosition.x;
        float totalDistance = distance * _spinCount;

        print("distance " + distance);
        print("totalDistance " + totalDistance);

        float previousDistance = 0;

        do
        {
            float lerpTime = expendedTime / _duration;
            float evaluated = _behaviourCurve.Evaluate(lerpTime);
            float currentDistance = Mathf.Lerp(0, totalDistance, evaluated);
            float xOffset = currentDistance - previousDistance;
            previousDistance = currentDistance;

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].anchoredPosition = _items[i].anchoredPosition.AddX(xOffset);

                if (replaceX < _items[i].anchoredPosition.x)
                {
                    _items[i].anchoredPosition = _items[0].anchoredPosition.AddX(-spacing);
                    ShiftArray();
                }
            }

            expendedTime += Time.deltaTime;

            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }
        while (expendedTime < _duration);
    }
}