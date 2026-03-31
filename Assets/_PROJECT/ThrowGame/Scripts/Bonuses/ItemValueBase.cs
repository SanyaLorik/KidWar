using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ItemValueBase<T> {
    [SerializeReference, SubclassSelector] public T Item;
    [Range(0,1), SerializeField] public float Weight;
}

public static class ItemValueBase {
    public static T GetRandomItemByWeight<T>(List<ItemValueBase<T>> itemWeight, float totalWeight) {
        float accumulated = 0;
        float choosedWeight = Random.Range(0, totalWeight);
        foreach (var modifierValue in itemWeight) {
            accumulated += modifierValue.Weight;
            if (accumulated > choosedWeight) {
                Debug.Log($"Выбивание веса {choosedWeight}, айтем: {modifierValue.Item.GetType()}");
                return modifierValue.Item;
            }
        }
        return itemWeight[^1].Item;
    }
}

