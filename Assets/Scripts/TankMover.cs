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
    private Vector3 _lookAtTarget;

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
            var targetRotation = Quaternion.LookRotation(_desiredLookAtTarget.GetValueOrDefault() - transform.position);
            var str = Mathf.Min(RotationSpeed * Time.deltaTime, 1f);
            var newRotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
            var diff = GetDiffAngle(transform.rotation, newRotation);
            transform.rotation = newRotation;

            // Correct sides are rotating but by wrong amount and looks odd.
            if (diff < 0f)
            {
                RotateLeftWheelsByValue(Mathf.Min(str * 100f, 5f));
            }
            else
            {
                RotateRightWheelsByValue(Mathf.Min(str * 100f, 5f));
            }

            if(Mathf.Abs(Angle(_desiredLookAtTarget.GetValueOrDefault())) < 2.0f)
            {
                _lookAtTarget = _desiredLookAtTarget.GetValueOrDefault();
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

    public void ProcessHorizontalAxisInput()
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
    }

    public void ProcessVericalAxisInput()
    {
        var verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
            RotateLeftWheelsByAxis("Vertical");
            RotateRightWheelsByAxis("Vertical");
        }
    }

    private void RotateLeftWheelsByAxis(string axis)
    {
        RotateLeftWheelsByValue(Input.GetAxis(axis) * 5.0f);
    }

    private void RotateLeftWheelsByValue(float value)
    {
        Debug.Log($"Rotate left wheels by {value}");
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
        Debug.Log($"Rotate right wheels by {value}");
        foreach (var curWheel in RightWheels)
        {
            curWheel.transform.Rotate(value, 0.0f, 0.0f);
        }
    }

    private float GetDiffAngle(
        Quaternion a,
        Quaternion b)
    {
        var forwardA = a * Vector3.forward;
        var forwardB = b * Vector3.forward;
        var angleA = Mathf.Atan2(forwardA.x, forwardA.z);
        var angleB = Mathf.Atan2(forwardB.x, forwardB.z);
        return Mathf.DeltaAngle(angleA, angleB);
    }
}
