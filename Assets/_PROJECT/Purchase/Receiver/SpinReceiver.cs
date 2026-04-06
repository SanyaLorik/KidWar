using LuringPlayer_M;
using System;
using UnityEngine;

[Serializable]
public class SpinReceiver : AwardReceiver
{
    [SerializeField] private WheelFortuneBase _wheelFortune;
    [SerializeField][Min(1)] private int _count = 3;

    public override void Receive()
    {
        _wheelFortune.AddSpin(_count);
    }
}