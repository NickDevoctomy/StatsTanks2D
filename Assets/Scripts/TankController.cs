using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
    public GameObject Turret;
    public float RotationSpeed = 2f;
    public float MovementSpeed = 8f;
    public float StartingCameraPivot = 0f;

    private Rigidbody _rigidBody;
    private GameObject[] _spawnPoints;
    private Transform _cameraPivot;
    private AudioSource _engineAudioSource;
    private AudioSource _turretAudioSource;
    private float _currentEngineAudioTime = 0f;
    private float _currentTurretAudioTime = 0f;
    private bool _fadeOut;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _engineAudioSource = GetComponent<AudioSource>();
        _turretAudioSource = Turret.GetComponent<AudioSource>();

        _spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        _cameraPivot = transform.Find("CameraPivot");
        _cameraPivot.rotation = Quaternion.Euler(StartingCameraPivot, 0, 0);
        MoveToRandomSpawnPoint();
    }

    void FixedUpdate()
    {
        // This stops the tank quantum tunneling through walls
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        DoMovement();

        var verticalAxisRaw = Input.GetAxisRaw("Vertical");
        var horizontalAxisRaw = Input.GetAxisRaw("Horizontal");
        bool hasAxisInput = verticalAxisRaw != 0 || horizontalAxisRaw != 0;
        AttackDecaySound(_engineAudioSource, hasAxisInput, ref _currentEngineAudioTime);

        DoTurretMovement();
        DoCameraPivot();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_rigidBody == null)
        {
            return;
        }

        // Take damage here
    }

    private void DoMovement()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            var rotation = horizontalAxis * RotationSpeed;
            transform.Rotate(new Vector3(0f, rotation, 0f));
        }

        var verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
            transform.Translate(new Vector3(0f, 0f, movement));
        }
    }

    private void AttackDecaySound(AudioSource audioSource, bool active, ref float currentTime)
    {
        if (active)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0f;
                currentTime = 0f;
                audioSource.Play();
            }
            else
            {
                _fadeOut = false;
                currentTime += Time.deltaTime;
                var newVolume = currentTime / 0.1f;
                audioSource.volume = newVolume;
            }
        }
        else
        {
            if (!_fadeOut &&
                audioSource.isPlaying)
            {
                currentTime = 0f;
                _fadeOut = true;
            }

            if (_fadeOut)
            {
                currentTime += Time.deltaTime;
                var newVolume = currentTime / 0.5f;
                audioSource.volume = 1.0f - newVolume;

                if (currentTime >= 0.5f)
                {
                    audioSource.Stop();
                    audioSource.volume = 0f;
                    currentTime = 0f;
                    _fadeOut = false;
                }
            }
        }
    }

    private void DoCameraPivot()
    {
        if (Input.GetKey(KeyCode.PageUp))
        {
            _cameraPivot.Rotate(Vector3.right * 15f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            _cameraPivot.Rotate(-Vector3.right * 15f * Time.deltaTime);
        }
    }

    private void DoTurretMovement()
    {
        var moved = false;
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

                // Move bot, just for testing at the moment
                if (Input.GetMouseButton(0))
                {
                    var botObject = GameObject.FindGameObjectWithTag("Bot");
                    if (botObject != null)
                    {
                        var bot = botObject.GetComponent<Bot>();
                        bot.CalculatePath(targetPosition);
                    }
                }
            }
        }

        AttackDecaySound(_turretAudioSource, moved, ref _currentTurretAudioTime);
    }

    private void MoveToRandomSpawnPoint()
    {
        var randomSpawnPointIndex = Random.Range(0, _spawnPoints.Length - 1);
        transform.position = _spawnPoints[randomSpawnPointIndex].transform.position;
    }
}
