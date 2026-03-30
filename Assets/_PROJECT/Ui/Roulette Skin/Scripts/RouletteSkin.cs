using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;

public class RouletteSkin : MonoBehaviour
{
    [SerializeField] private int _count;
    [SerializeField] private float _duration;
    [SerializeField] private AnimationCurve _behaviour;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform[] _items;

    private float _spacing;

    private void Start()
    {
        _spacing = _items[1].anchoredPosition.x - _items[0].anchoredPosition.x;

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

        float borderX = _rect.anchoredPosition.x + _rect.rect.width;
        float replaceX = borderX + _spacing;

        do
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].anchoredPosition = _items[i].anchoredPosition.AddX(200 * Time.deltaTime);

                if (replaceX < _items[i].anchoredPosition.x)
                {
                    _items[i].anchoredPosition = _items[0].anchoredPosition.AddX(-_spacing);
                    ShiftArray();
                }
            }

            expendedTime += Time.deltaTime;

            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }
        while (expendedTime < _duration);
    }
}