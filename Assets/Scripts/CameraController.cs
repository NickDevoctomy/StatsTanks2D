using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject CameraXPivot;
    public GameObject CameraYPivot;
    public float RotationSpeed = 1f;
    public float MinVerticalRotation = -65.0f;
    public float MaxVerticalRotation = 8f;

    private float? _desiredXRotation;
    private float? _desiredYRotation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHorizontalAxisInput();
        ProcessVericalAxisInput();

        if (_desiredXRotation.HasValue)
        {
            CameraYPivot.transform.Rotate(0f, _desiredXRotation.GetValueOrDefault(), 0f, Space.Self);
            _desiredXRotation = null;
        }

        if (_desiredYRotation.HasValue)
        {
            CameraXPivot.transform.Rotate(_desiredYRotation.GetValueOrDefault(), 0f, 0f, Space.Self);

            // Prevent rotation outside of these bounds
            var eulerRotation = CameraXPivot.transform.eulerAngles;
            float normalizedX = NormalizeAngle(eulerRotation.x);
            if (normalizedX > MaxVerticalRotation || normalizedX < MinVerticalRotation)
            {
                normalizedX = Mathf.Clamp(normalizedX, MinVerticalRotation, MaxVerticalRotation);
                eulerRotation.x = normalizedX;
                CameraXPivot.transform.rotation = Quaternion.Euler(eulerRotation);
            }

            _desiredYRotation = null;
        }
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    public bool ProcessHorizontalAxisInput()
    {
        var horizontalAxis = Input.GetAxis(Globals.HorizontalCameraInputName);
        if (horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed; 
            _desiredXRotation = rotation;
        }

        return horizontalAxis != 0;
    }

    public bool ProcessVericalAxisInput()
    {
        var verticalAxis = Input.GetAxis(Globals.VerticalCameraInputName);
        if (verticalAxis != 0)
        {
            var rotation = verticalAxis * RotationSpeed;
            _desiredYRotation = rotation;
        }

        return verticalAxis != 0;
    }
}
