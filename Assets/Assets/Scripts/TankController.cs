using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject Turret;
    public float RotationSpeed = 2f;
    public float MovementSpeed = 8f;

    private Rigidbody _rigidBody;
    private GameObject[] _spawnPoints;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        MoveToRandomSpawnPoint();
    }

    void FixedUpdate()
    {
        // This stops the tank quantum tunneling through walls
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        var horizontalAxis = Input.GetAxis("Horizontal");
        if(horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed;
            transform.Rotate(new Vector3(0f, rotation, 0f));
        }

        var verticalAxis = Input.GetAxis("Vertical");
        if(verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
        }

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out var cameraRayHit))
        {
            if (cameraRayHit.transform.tag == "Floor")
            {
                var targetPosition = new Vector3(
                    cameraRayHit.point.x,
                    Turret.transform.position.y,
                    cameraRayHit.point.z);
                Turret.transform.LookAt(targetPosition);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_rigidBody == null)
        {
            return;
        }

        // Take damage here
    }

    private void MoveToRandomSpawnPoint()
    {
        var randomSpawnPointIndex = Random.Range(0, _spawnPoints.Length - 1);
        transform.position = _spawnPoints[randomSpawnPointIndex].transform.position;
    }
}
