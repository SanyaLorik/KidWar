using System;
using UnityEngine;

[Serializable]
public struct StaticTranslation<T>
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public T Data { get; private set; }
}
