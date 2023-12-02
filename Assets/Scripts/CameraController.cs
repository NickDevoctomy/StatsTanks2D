using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotationSpeed = 1f;

    private Camera _mainCamera;
    private float? _desiredXRotation;
    private float? _desiredYRotation;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHorizontalAxisInput();
        ProcessVericalAxisInput();

        if (_desiredXRotation.HasValue)
        {
            _mainCamera.transform.Rotate(new Vector3(0f, _desiredXRotation.GetValueOrDefault(), 0f));
            _desiredXRotation = null;
        }

        if (_desiredYRotation.HasValue)
        {
            _mainCamera.transform.Rotate(new Vector3(_desiredYRotation.GetValueOrDefault(), 0f, 0f));
            _desiredYRotation = null;
        }
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
