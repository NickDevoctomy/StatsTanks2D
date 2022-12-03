using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject Turret;
    public float RotationSpeed = 2f;
    public float MovementSpeed = 8f;

    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        if(horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed;
            transform.Rotate(new Vector3(0f, rotation, 0f));
        }
        else
        {
            _rigidBody.angularVelocity = Vector3.zero;
        }

        var verticalAxis = Input.GetAxis("Vertical");
        if(verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
        }
        else
        {
            _rigidBody.velocity = Vector3.zero;
        }

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(
            mousePosition.x,
            Turret.transform.position.y,
            mousePosition.z);
        Turret.transform.LookAt(
            mousePosition,
            transform.up);
    }

    void OnCollisionEnter(Collision collision)
    {
        _rigidBody.angularVelocity = Vector3.zero;
        _rigidBody.velocity = Vector3.zero;
    }
}
