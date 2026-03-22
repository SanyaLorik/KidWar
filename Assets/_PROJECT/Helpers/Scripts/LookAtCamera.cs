using UnityEngine;

public class LookAtCamera : MonoBehaviour { 
    [SerializeField] private Mode mode;


    private enum Mode {
        LookAt,
        LookAtInvert,
        CameraForward,
        CameraBackward,
    }

    private void LateUpdate() { 
        switch (mode) {
          case Mode.LookAt:
            transform.LookAt(Camera.main.transform);
            break;

        case Mode.LookAtInvert:
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0,180,0);
            break;

        case Mode.CameraForward:
            transform.forward = Camera.main.transform.forward; 
            break;

        case Mode.CameraBackward:
            transform.forward = -Camera.main.transform.forward;
            break;
     }
   }
}
