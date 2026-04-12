using System;
using UnityEngine;

[Serializable]
public struct StaticTranslation<T>
{
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public T Data { get; set; }
}
