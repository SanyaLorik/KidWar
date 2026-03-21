using Architecture_M;
using UnityEngine;

public class MobileInput : MobileInputBase<MobileInputView>, IOrbitalRotationInput, IActivityButtonPC
{
    public MobileInput(MobileInputView inputView) : base(inputView)
    {

    }

    public Vector2 OrbitalDirection => inputView.OrbitalDirection;

    public override void Enable()
    {
        inputView.JumpButton.onClick.AddListener(OnInvokedJump);

        inputView.Enable();
    }

    public override void Disable()
    {
        inputView.JumpButton.onClick.RemoveListener(OnInvokedJump);

        inputView.Disable();
    }

    private void OnInvokedJump()
    {
        InvokeJump();
    }

    public void ShowJumpButton()
    {
        inputView.ShowJumpButton();
    }

    public void HideJumpButton()
    {
        inputView.HideJumpButton();
    }

    public void ShowOrbitalJoystick()
    {
        inputView.ShowOrbitalJoystick();
    }

    public void HidOrbitalJoystick()
    {
        inputView.HidOrbitalJoystick();
    }
}