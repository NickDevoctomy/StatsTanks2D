using UnityEngine;

public class TankMover : MonoBehaviour
{
    public GameObject Turret;
    public GameObject[] LeftWheels;
    public GameObject[] RightWheels;
    public float RotationSpeed = 2f;
    public float MovementSpeed = 8f;
    public float VirtualHorizontalAxis = 0f;
    public float VirtualVerticalAxis = 0f;

    private float? _desiredYRotation;
    private Vector3? _desiredLookAtTarget;
    private Vector3? _lookAtTarget;
    private Vector3? _desiredMoveToTarget;

    void Start()
    {
    }

    void Update()
    {
        // Do I need both of these rotation blocks, can it be done by a single one?
        if (_desiredYRotation.HasValue)
        {
            transform.Rotate(new Vector3(0f, _desiredYRotation.GetValueOrDefault(), 0f));
            _desiredYRotation = null;
        }

        if(_desiredLookAtTarget.HasValue)
        {
            var angleToNext = Angle(_desiredLookAtTarget.GetValueOrDefault());
            if (angleToNext < 0f)
            {
                var axisAmout = Mathf.Abs(angleToNext) / 45f;
                PerformVirtualHorizontalAxis(axisAmout > 1f ? 1f : axisAmout);
            }
            else
            {
                var axisAmout = Mathf.Abs(angleToNext) / 45f;
                PerformVirtualHorizontalAxis(axisAmout > 1f ? -1f : -axisAmout);
            }

            var resultingAngle = Mathf.Abs(Angle(_desiredLookAtTarget.GetValueOrDefault()));
            if (resultingAngle < 0.15f)
            {
                _lookAtTarget = _desiredLookAtTarget.GetValueOrDefault();
                _desiredLookAtTarget = null;
                PerformVirtualHorizontalAxis(0f);
            }
        }

        if (_desiredMoveToTarget.HasValue)
        {
            var distance = Vector3.Distance(transform.position, _desiredMoveToTarget.GetValueOrDefault());
            var axisAmout = distance / 5f;
            PerformVirtualVerticalAxis(axisAmout > 1f ? 1f : axisAmout);
        }
    }

    // This stops the tank quantum tunneling through walls
    public void ForceLevel()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    public float Angle(Vector3 position)
    {
        var angleToNext = Vector3.SignedAngle(
               position - transform.position,
               transform.forward,
               Vector3.up);
        return angleToNext;
    }

    public void LookAt(
        Vector3 position,
        bool forceEyeline,
        out bool isCurrentTarget)
    {
        var pos = forceEyeline ?
                new Vector3(position.x, transform.position.y, position.z) :
                position;
        if (_desiredLookAtTarget != pos &&
            _lookAtTarget != pos)
        {
            _desiredLookAtTarget = pos;
        }

        isCurrentTarget = _lookAtTarget == pos;
    }

    public void StopRotate()
    {
        _desiredLookAtTarget = null;
    }

    public void StopMove()
    {
        _desiredMoveToTarget = null;
    }

    public bool ProcessHorizontalAxisInput()
    {
        var horizontalAxis = Input.GetAxis(Globals.HorizontalMovementInputName);
        if (horizontalAxis != 0)
        {
            Debug.Log("Horizontal movement input detected.");
            var rotation = horizontalAxis * RotationSpeed;
            _desiredYRotation = rotation;

            if (horizontalAxis > 0)
            {
                RotateRightWheelsByAxis(Globals.HorizontalMovementInputName);
            }
            else
            {
                RotateLeftWheelsByAxis(Globals.HorizontalMovementInputName);
            }
        }

        return horizontalAxis != 0;
    }

    public bool ProcessVericalAxisInput()
    {
        var verticalAxis = Input.GetAxis(Globals.VerticalMovementInputName);
        if (verticalAxis != 0)
        {
            Debug.Log("Vertical movement input detected.");
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
            RotateLeftWheelsByAxis(Globals.VerticalMovementInputName);
            RotateRightWheelsByAxis(Globals.VerticalMovementInputName);
        }

        return verticalAxis != 0;
    }

    private void PerformVirtualHorizontalAxis(float value)
    {
        VirtualHorizontalAxis = value;
        var horizontalAxis = value; // Need to apply curve to this
        var rotation = horizontalAxis * RotationSpeed;
        _desiredYRotation = rotation;
        RotateLeftWheelsByValue(horizontalAxis * 5.0f);
    }

    public void PerformVirtualVerticalAxis(float value)
    {
        VirtualVerticalAxis = value;
        var verticalAxis = value;
        var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
        transform.Translate(new Vector3(0f, 0f, movement));
        RotateLeftWheelsByValue(verticalAxis * 5.0f);
        RotateRightWheelsByValue(verticalAxis * 5.0f);
    }

    private void RotateLeftWheelsByAxis(string axis)
    {
        RotateLeftWheelsByValue(Input.GetAxis(axis) * 5.0f);
    }

    private void RotateLeftWheelsByValue(float value)
    {
        foreach (var curWheel in LeftWheels)
        {
            curWheel.transform.Rotate(value, 0.0f, 0.0f);
        }
    }

    private void RotateRightWheelsByAxis(string axis)
    {
        RotateRightWheelsByValue(Input.GetAxis(axis) * 5.0f);
    }

    private void RotateRightWheelsByValue(float value)
    {
        foreach (var curWheel in RightWheels)
        {
            curWheel.transform.Rotate(value, 0.0f, 0.0f);
        }
    }
}
