using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputView : MobileInputViewBase
{
    [field: SerializeField] public Button JumpButton { get; private set; }
    [SerializeField] private InputHandle _orbital;

    public Vector2 OrbitalDirection => _orbital.Direction;

    public override void Enable()
    {
        base.Enable();

        _orbital.ActiveSelf();
    }

    public override void Disable()
    {
        base.Disable();

        _orbital.DisactiveSelf();
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
        _orbital.ActiveSelf();
    }

    public void HidOrbitalJoystick()
    {
        _orbital.DisactiveSelf();
    }
}
