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
    [SerializeField] private RectTransform _finish;

    private float _spacing;
    private float _distance;

    private void Awake()
    {
        _spacing = _items[1].Rect.anchoredPosition.x - _items[0].Rect.anchoredPosition.x;
        _distance = (_items[^1].Rect.anchoredPosition.x + _spacing) - _items[0].Rect.anchoredPosition.x;

        _items.ForEach(i => i.StorePosition());

        //UniTask.Create(async () =>
        //{
        //    while (destroyCancellationToken.IsCancellationRequested == false)
        //    {
        //        FillRandomSkins();
        //        await SpinAsync();

        //        ResetItemPositions();

        //        await UniTask.Delay(2000);
        //    }
        //});
    }

    public async UniTask<ThrowableObject> SpinAsync()
    {
        ThrowableObject throwableObject = GetRandomThrowableObjectByChance();

        print("RouletteSkin: " + throwableObject);

        float expendedTime = 0;

        float randomOffset = _distance * UnityEngine.Random.Range(_spinRangePercent.From, _spinRangePercent.To);
        float totalDistance = _distance * _spinCount + randomOffset;

        print("_distance: " + _distance);
        print("totalDistance: " + totalDistance);

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

                if (_finish.anchoredPosition.x <= _items[i].Rect.anchoredPosition.x)
                {
                    int indexNextItem = (i + 1) % _items.Length;
                    _items[i].Rect.anchoredPosition = _items[indexNextItem].Rect.anchoredPosition.AddX(-_spacing);

                    ChangeInfo(_items[i]);

                    bool isLastSpin = (totalDistance - currentDistance) <= _distance;
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
        _items.ForEach(i => i.ResetPosition());
    }

    private void ChangeInfo(RouletteSkinItem rouletteSkinItem)
    {
        InfoThrowableObject randomInfo = _infoThrowableObjects.GetRandomElement().Info;
        rouletteSkinItem.SetInfo(randomInfo);
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