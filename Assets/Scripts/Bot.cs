using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rigidBody;
    private MultiSampleAudioPlayer _audioPlayer;
    private bool _forceSharpTurn;
    private Vector3 _turnTarget;
    private bool _isSharpTurning;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioPlayer = GetComponent<MultiSampleAudioPlayer>();
    }

    void Update()
    {
        var distanceToEnd = Vector3.Distance(transform.position, _navMeshAgent.destination);
        if(distanceToEnd > 3)
        {
            if (!_isSharpTurning &&
                !_navMeshAgent.pathPending &&
                _navMeshAgent.path.corners.Length > 1)
            {
                var angleToNext = GetAngleToNext(null, out var nextCorner);
                if (!_forceSharpTurn &&
                    Mathf.Abs(angleToNext) > 90)
                {
                    // Our angle to next position is too sharp, and we don't want to drift
                    // so let's stop navigation and perform a sharp turn
                    _forceSharpTurn = true;
                    _navMeshAgent.isStopped = true;
                    _turnTarget = nextCorner;
                    _rigidBody.velocity = Vector3.zero;
                    _rigidBody.angularVelocity = Vector3.zero;
                    _isSharpTurning = true;
                }
            }

            if (_forceSharpTurn &&
                _navMeshAgent.path.corners.Length > 1)
            {
                var angleToNext = GetAngleToNext(_turnTarget, out var nextCorner);
                if (Mathf.Abs(angleToNext) > 5)
                {
                    // Sharp turn toward next nav position
                    var targetRotation = Quaternion.LookRotation(_turnTarget - transform.position);
                    var str = Mathf.Min(2.0f * Time.deltaTime, 1);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
                    _audioPlayer.PlayWithAttackAndRelease("Engine", true);
                }
                else
                {
                    // We finished our turn, let's resume navigation
                    _isSharpTurning = false;
                    _navMeshAgent.isStopped = false;
                    _forceSharpTurn = false;
                    _audioPlayer.PlayWithAttackAndRelease("Engine", false);
                }
            }
        }


        float velocity = _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
        var isMoving = velocity > 0.1f;
        _audioPlayer.PlayWithAttackAndRelease("Engine", isMoving);
        _audioPlayer.PlayWithAttackAndRelease("EngineIdle", !isMoving);
    }

    void OnDrawGizmos()
    {
        if (_navMeshAgent != null && _navMeshAgent?.path != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _navMeshAgent.path.corners.Length - 1; i++)
            {
                var curCorner = new Vector3(_navMeshAgent.path.corners[i].x, transform.position.y, _navMeshAgent.path.corners[i].z);
                var nextCorner = new Vector3(_navMeshAgent.path.corners[i+1].x, transform.position.y, _navMeshAgent.path.corners[i+1].z);
                Gizmos.DrawSphere(nextCorner, 0.5f);
                Gizmos.DrawLine(
                    curCorner,
                    nextCorner);
            }
        }
    }

    private float GetAngleToNext(Vector3? nextCorner, out Vector3 usedNextCorner)
    {
        var forwardVector = transform.position + transform.forward * 2;
        usedNextCorner = nextCorner.HasValue ? nextCorner.GetValueOrDefault() : new Vector3(_navMeshAgent.path.corners[1].x, transform.position.y, _navMeshAgent.path.corners[1].z);
        var side1 = forwardVector - transform.position;
        var side2 = usedNextCorner - transform.position;
        return Vector3.Angle(side1, side2);
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        _isSharpTurning = false;
        _forceSharpTurn = false;
        _navMeshAgent.SetDestination(targetPosition);
        _navMeshAgent.isStopped = false;
    }

    public void PositionOnNavMesh()
    {
        if (NavMesh.SamplePosition(
            transform.position,
            out var closestHit,
            500f,
            NavMesh.AllAreas))
        {
            if(_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            transform.position = closestHit.position;
        }
    }
}
