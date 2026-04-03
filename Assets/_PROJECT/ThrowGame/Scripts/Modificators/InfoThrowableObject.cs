using System;
using UnityEngine;

[Serializable]
public class InfoThrowableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField][field: Min(0)] public int Damage { get; private set; }
    [field: SerializeField][field: Range(0, 1)] public float DropChance { get; private set; }
    [field: SerializeField]public Color RareColor { get; private set; }
}