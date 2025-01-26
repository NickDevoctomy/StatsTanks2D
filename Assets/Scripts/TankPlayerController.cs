using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TankPlayerController : MonoBehaviour
{
    public GameObject Turret;
    public GameObject[] LeftWheels;
    public GameObject[] RightWheels;
    public float MovementSpeed = 8f;
    public float StartingCameraPivot = 0f;

    private TankMover _tankMover;
    private Rigidbody _rigidBody;
    private GameObject[] _spawnPoints;
    private MultiSampleAudioPlayer _audioPlayer;

    private Transform _canon;
    private float _currentCanonAngle = 0f;

    void Start()
    {
        _tankMover = GetComponent<TankMover>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioPlayer = GetComponent<MultiSampleAudioPlayer>();

        _canon = transform.Find("TankFree_Blue/TankFree_Tower/TankFree_Canon");

        _spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        MoveToRandomSpawnPoint();
    }

    void FixedUpdate()
    {
        DoMovement();
        PlayeEngineSounds();
        DoTurretMovement();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_rigidBody == null)
        {
            return;
        }

        // Take damage here
    }

    private void PlayeEngineSounds()
    {
        var verticalAxisRaw = Input.GetAxisRaw(Globals.VerticalMovementInputName);
        var horizontalAxisRaw = Input.GetAxisRaw(Globals.HorizontalMovementInputName);
        bool hasAxisInput = verticalAxisRaw != 0 || horizontalAxisRaw != 0;
        _audioPlayer.PlayWithAttackAndRelease("Engine", hasAxisInput);
        _audioPlayer.PlayWithAttackAndRelease("EngineIdle", !hasAxisInput);
    }

    private void DoMovement()
    {
        _tankMover.ForceLevel();
        var horizontalInput = _tankMover.ProcessHorizontalAxisInput();
        var verticalInput = _tankMover.ProcessVericalAxisInput();
        if(!horizontalInput && !verticalInput)
        {
            _rigidBody.linearVelocity = Vector3.zero;
        }
    }

    private void DoTurretMovement()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return; // Exit if no gamepad is connected

        var rotationSpeed = 20f;
        var leftTrigger = Gamepad.current[GamepadButton.LeftShoulder];
        if (leftTrigger.IsPressed())
        {
            _currentCanonAngle = Mathf.Clamp(_currentCanonAngle - rotationSpeed * Time.deltaTime, -45, 20);
            _canon.localEulerAngles = new Vector3(_currentCanonAngle, _canon.localEulerAngles.y, _canon.localEulerAngles.z);
        }

        var rightTrigger = Gamepad.current[GamepadButton.RightShoulder];
        if (rightTrigger.IsPressed())
        {
            _currentCanonAngle = Mathf.Clamp(_currentCanonAngle + rotationSpeed * Time.deltaTime, -45, 20);
            _canon.localEulerAngles = new Vector3(_currentCanonAngle, _canon.localEulerAngles.y, _canon.localEulerAngles.z);
        }


        // Mouse aiming
        /*var moved = false;
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out var cameraRayHit))
        {
            if (cameraRayHit.transform.tag == "Floor")
            {
                var targetPosition = new Vector3(
                    cameraRayHit.point.x,
                    Turret.transform.position.y,
                    cameraRayHit.point.z);

                var targetRotation = Quaternion.LookRotation(targetPosition - Turret.transform.position);
                var angleToRotate = Quaternion.Angle(Turret.transform.rotation, targetRotation);
                moved = angleToRotate > 10;

                var str = Mathf.Min(2.0f * Time.deltaTime, 1);
                Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, targetRotation, str);
            }
        }

        _audioPlayer.PlayWithAttackAndRelease("Turret", moved);*/
    }

    private void MoveToRandomSpawnPoint()
    {
        var randomSpawnPointIndex = Random.Range(0, _spawnPoints.Length - 1);
        transform.position = _spawnPoints[randomSpawnPointIndex].transform.position;
    }
}
