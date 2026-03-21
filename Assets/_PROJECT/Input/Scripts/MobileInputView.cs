using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputView : MobileInputViewBase
{
    [field: SerializeField] public Button JumpButton { get; private set; }

    [SerializeField] private Joystick _orbitalMovement;

    public Vector2 OrbitalDirection => _orbitalMovement.Direction;

    public override void Enable()
    {
        base.Enable();

        _orbitalMovement.ActiveSelf();
    }

    public override void Disable()
    {
        base.Disable();

        _orbitalMovement.DisactiveSelf();
    }

    public void ShowJumpButton()
    {
        JumpButton.ActiveSelf();
    }

    public void HideJumpButton()
    {
        JumpButton.DisactiveSelf();
    }

    public void ShowOrbitalJoystick()
    {
        _orbitalMovement.ActiveSelf();
    }

    public void HidOrbitalJoystick()
    {
        _orbitalMovement.DisactiveSelf();
    }
}