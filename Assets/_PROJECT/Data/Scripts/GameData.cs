using System;
using SanyaBeerExtension;
using UnityEngine;

[Serializable]
public class GameData : GameDataBase
{
    [field: Header("Player Movement")]
    [field: SerializeField] public float WalkSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float SecondJumpForce { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
    [field: SerializeField] public float GravityScale { get; private set; }
    
    [field: Header("Camera")]
    [field: Header("Дефолтные значения в процентах")]
    [field: SerializeField, Range(0,1)] public float MobileCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DesktopCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float FlightCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DefaultCameraSens { get; private set; }
    [field: SerializeField, Range(0,1)] public float ZoomSpeed { get; private set; }
    
    [field: Header("Множители сенсы")]
    [field: SerializeField] public float JoystickSensivityMultiplier  { get; private set; }
    [field: SerializeField] public float MouseSensivityMultiplier { get; private set; }
    
    [field: Header("Ограничители")]
    [field: SerializeField] public PairedValue<float> ZoomDiapasone  { get; private set; }
    [field: SerializeField] public float MinSensValue  { get; private set; }
    
    
    
}