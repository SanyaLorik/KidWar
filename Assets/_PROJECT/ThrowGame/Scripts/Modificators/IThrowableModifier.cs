public interface IThrowableModifier {
    public ThrowableObject ThrowableObject { get;}
    public void SetThrowableObject(ThrowableObject throwableObject);
    public void ExtensionBehaviour();
    public void OnPlayerContact();
    public void CalculatePose(float elapsedTime);
}