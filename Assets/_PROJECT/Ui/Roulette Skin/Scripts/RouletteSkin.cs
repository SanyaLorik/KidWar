using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouletteSkin : MonoBehaviour
{
    [Header("Parametrs")]
    [SerializeField] private AnimationCurve _behaviourCurve;
    [SerializeField] private int _spinCount;
    [SerializeField] private float _duration;
    [SerializeField] private PairedValue<float> _spinRangePercent;

    [Header("Views")]
    [SerializeField] private RouletteSkinItem _selectedItem;
    [SerializeField] private RouletteSkinItem[] _items;

    [Header("Pets")]
    [SerializeField] private ThrowableObject[] _infoThrowableObjects;

    [Header("Other")]
    [SerializeField] private RectTransform _rect;

    [ContextMenu("Spin")]
    private void SpinInInspector()
    {
        ResetItemPosition();
        FillRandomSkins();
        SpinAsync().Forget();
    }

    public async UniTask<ThrowableObject> SpinAsync()
    {
        ThrowableObject throwableObject = GetRandomThrowableObjectByChance();

        print("throwableObject " + throwableObject);

        float expendedTime = 0;

        float spacing = _items[1].Rect.anchoredPosition.x - _items[0].Rect.anchoredPosition.x;
        float borderX = _rect.anchoredPosition.x + _rect.rect.width;
        float replaceX = borderX + spacing;

        float distance = (_items[^1].Rect.anchoredPosition.x + spacing) - _items[0].Rect.anchoredPosition.x;
        float randomOffset = distance * UnityEngine.Random.Range(_spinRangePercent.From, _spinRangePercent.To);
        float totalDistance = distance * _spinCount + randomOffset;

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
                _items[i].Rect.anchoredPosition = _items[i].Rect.anchoredPosition.AddX(xOffset);

                if (replaceX < _items[i].Rect.anchoredPosition.x)
                {
                    _items[i].Rect.anchoredPosition = _items[0].Rect.anchoredPosition.AddX(-spacing);
                    ShiftArray();
                    ChangeInfoAtFirstItem();

                    bool isLastSpin = (totalDistance - currentDistance) <= distance;
                    if (isLastSpin)
                        _selectedItem.SetInfo(throwableObject.Info);
                }
            }

            expendedTime += Time.deltaTime;

            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }
        while (expendedTime < _duration);

        return throwableObject;
    }

    public void FillRandomSkins()
    {
        foreach (RouletteSkinItem item in _items)
        {
            InfoThrowableObject randomInfo = _infoThrowableObjects.GetRandomElement().Info;
            item.SetInfo(randomInfo);
        }
    }

    public void ResetItemPosition()
    {
        _items.ForEach(i => i.Rect.anchoredPosition = i.InitialPosition);
    }

    private void ShiftArray()
    {
        RouletteSkinItem last = _items[^1];
        for (int i = _items.Length - 1; i >= 1; i--)
            _items[i] = _items[i - 1];

        _items[0] = last;
    }

    private void ChangeInfoAtFirstItem()
    {
        InfoThrowableObject randomInfo = _infoThrowableObjects.GetRandomElement().Info;
        RouletteSkinItem item = _items[0];
        item.SetInfo(randomInfo);
    }

    private ThrowableObject GetRandomThrowableObjectByChance()
    {
        float totalChance = _infoThrowableObjects.Sum(i => i.Info.DropChance);
        IEnumerable<float> normalizedChances = _infoThrowableObjects.Select(i => i.Info.DropChance / totalChance);

        float cumulativeSum = 0;
        IReadOnlyList<float> cumulativeChances = normalizedChances.Select(i =>
        {
            cumulativeSum += i;
            return cumulativeSum;
        }).ToList();

        float randomChance = UnityEngine.Random.value;

        for (int i = 0; i < cumulativeChances.Count; i++)
        {
            if (randomChance < cumulativeChances[i])
                return _infoThrowableObjects[i];
        }

        throw new Exception("WTF?");
    }
}