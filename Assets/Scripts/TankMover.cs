using UnityEngine;

public class TankMover : MonoBehaviour
{
    public GameObject Turret;
    public GameObject[] LeftWheels;
    public GameObject[] RightWheels;
    public float RotationSpeed = 2f;
    public float MovementSpeed = 8f;

    private float? _desiredYRotation;
    private Vector3? _desiredLookAtTarget;
    private Vector3? _lookAtTarget;
    private Vector3? _desiredMoveToTarget;
    private Vector3? _moveToTarget;

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
            if (resultingAngle < 0.1f)
            {
                Debug.Log($"Resulting angle = {resultingAngle}");
                _lookAtTarget = _desiredLookAtTarget.GetValueOrDefault();
                _desiredLookAtTarget = null;
            }
        }

        if (_desiredMoveToTarget.HasValue)
        {
            var distance = Vector3.Distance(transform.position, _desiredMoveToTarget.GetValueOrDefault());
            if (distance > 0.35f)
            {
                Debug.Log($"Moving to target, distance = {distance}");
                var axisAmout = distance / 5f;
                PerformVirtualVerticalAxis(axisAmout > 1f ? 1f : axisAmout);
            }
            else
            {
                _moveToTarget = _desiredMoveToTarget;
                _desiredMoveToTarget = null;
                _desiredLookAtTarget = null;
            }
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

    public void MoveTo(
        Vector3 position,
        bool forceEyeline,
        out bool isCurrentTarget)
    {
        var pos = forceEyeline ?
                new Vector3(position.x, transform.position.y, position.z) :
                position;
        if (_desiredMoveToTarget != pos &&
            _moveToTarget != pos)
        {
            _desiredMoveToTarget = pos;
        }

        isCurrentTarget = _moveToTarget == pos;
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
        var horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed;
            _desiredYRotation = rotation;
            if (horizontalAxis > 0)
            {
                RotateRightWheelsByAxis("Horizontal");
            }
            else
            {
                RotateLeftWheelsByAxis("Horizontal");
            }
        }

        return horizontalAxis != 0;
    }

    public bool ProcessVericalAxisInput()
    {
        var verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
            RotateLeftWheelsByAxis("Vertical");
            RotateRightWheelsByAxis("Vertical");
        }

        return verticalAxis != 0;
    }

    private void PerformVirtualHorizontalAxis(float value)
    {
        var horizontalAxis = value; // Need to apply curve to this
        var rotation = horizontalAxis * RotationSpeed;
        _desiredYRotation = rotation;
        RotateLeftWheelsByValue(horizontalAxis * 5.0f);
    }

    public void PerformVirtualVerticalAxis(float value)
    {
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
