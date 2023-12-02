using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotationSpeed = 1f;

    private Transform _cameraYPivot;
    private Transform _cameraXPivot;
    private float? _desiredXRotation;
    private float? _desiredYRotation;
    private float _xMin = -65.0f;
    private float _xMax = 8f;

    // Start is called before the first frame update
    void Start()
    {
        _cameraYPivot = GameObject.Find("CameraYPivot")?.transform;
        _cameraXPivot = GameObject.Find("CameraXPivot")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHorizontalAxisInput();
        ProcessVericalAxisInput();

        if (_desiredXRotation.HasValue)
        {
            _cameraYPivot.Rotate(0f, _desiredXRotation.GetValueOrDefault(), 0f, Space.Self);
            _desiredXRotation = null;
        }

        if (_desiredYRotation.HasValue)
        {
            _cameraXPivot.Rotate(_desiredYRotation.GetValueOrDefault(), 0f, 0f, Space.Self);

            // Prevent rotation outside of these bounds
            var eulerRotation = _cameraXPivot.eulerAngles;
            float normalizedX = NormalizeAngle(eulerRotation.x);
            if (normalizedX > _xMax || normalizedX < _xMin)
            {
                normalizedX = Mathf.Clamp(normalizedX, _xMin, _xMax);
                eulerRotation.x = normalizedX;
                _cameraXPivot.rotation = Quaternion.Euler(eulerRotation);
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
            Debug.Log("Horizontal camera input detected.");
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
            Debug.Log("Vertical camera input detected.");
            var rotation = verticalAxis * RotationSpeed;
            _desiredYRotation = rotation;
        }

        return verticalAxis != 0;
    }
}
