using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
            var rotation = -horizontalAxis * RotationSpeed;
            transform.Rotate(new Vector3(0f, 0f, rotation));
        }
        else
        {
            _rigidBody.angularVelocity = Vector3.zero;
        }

        var verticalAxis = Input.GetAxis("Vertical");
        if(verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, movement, 0f));
        }
        else
        {
            _rigidBody.velocity = Vector3.zero;
        }

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, Turret.transform.position.z);
        Turret.transform.LookAt(mousePosition, -transform.forward);
    }

    void OnCollisionEnter(Collision collision)
    {
        _rigidBody.angularVelocity = Vector3.zero;
        _rigidBody.velocity = Vector3.zero;
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
