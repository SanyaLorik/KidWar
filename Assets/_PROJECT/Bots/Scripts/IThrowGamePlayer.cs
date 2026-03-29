using UnityEngine;

public interface IThrowGamePlayer {
    public void TpInPoint(Vector3 pos);
    public void RotateToTarget(Vector3 targetPosition);
    public void SetPlayStatus(bool goPlay);

    public ObjectThrower ObjectThrower { get; }
    public bool IsPlaying { get; }
}