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
    [field: Header("Player Movement")]
    [field: SerializeField] public int PlayerMaxHp { get; private set; }
    
    
    [field: Header("Camera")]
    [field: Header("Дефолтные значения в процентах")]
    [field: SerializeField, Range(0,1)] public float MobileCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DesktopCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float FlightCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DefaultCameraSens { get; private set; }
    [field: SerializeField, Range(0,1)] public float ZoomSpeed { get; private set; }
    [field: Header("Для броска значение осей")]
    [field: SerializeField] public float HorizontalAxisValueToThrow { get; private set; }
    [field: SerializeField] public float VerticalAxisValueToThrow { get; private set; }
    [field: Header("Приближение во время перекидывания")]
    [field: SerializeField, Range(0,1)] public float ZoomPercentInThrowGame { get; private set; }
    [field: SerializeField] public PairedValue<float> LeftPlayerWinnerAxis { get; private set; }
    [field: SerializeField] public PairedValue<float> RightPlayerWinnerAxis { get; private set; }
    [field: SerializeField, Range(0,1)] public float ZoomToWinnerView { get; private set; }
    
    
    
    
    [field: Header("Множители сенсы")]
    [field: SerializeField] public float JoystickSensivityMultiplier  { get; private set; }
    [field: SerializeField] public float MouseSensivityMultiplier { get; private set; }
    
    [field: Header("Ограничители")]
    [field: SerializeField] public PairedValue<float> ZoomDiapasone  { get; private set; }
    [field: SerializeField] public float MinSensValue  { get; private set; }
    
    // Боты (потом уберу ненужное)
    [field: Header("БОТЫ")]
    [field: SerializeField] public PairedValue<int> CountSpeakingBotsPerTime  { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToSpeak { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToChangeSkin { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToChangeNickname { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToStayAfterSpawn { get; private set; }

    
    [field: Header("Movement")]
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanceToJump { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToGoPlayer { get; private set; }
    [field: SerializeField] public PairedValue<float> StoppingDistance { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToStayOnPoint { get; private set; }
    
    
}