using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TankBotController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private MultiSampleAudioPlayer _audioPlayer;
    private TankMover _tankMover;
    private List<Vector3> _path = new List<Vector3>();

    private Vector3? _facingPosition;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioPlayer = GetComponent<MultiSampleAudioPlayer>();
        _tankMover = GetComponent<TankMover>();
    }

    void Update()
    {
        DoMovement();
    }

    void OnDrawGizmos()
    {
        if (_navMeshAgent != null &&
            _path.Count > 0)
        {
            Gizmos.color = Color.green;
            var firstCorner = new Vector3(_path[0].x, transform.position.y, _path[0].z);
            Gizmos.DrawLine(
                transform.position,
                firstCorner);
            Gizmos.DrawSphere(firstCorner, 0.5f);
            for (int i = 0; i < _path.Count - 1; i++)
            {
                var curCorner = new Vector3(_path[i].x, transform.position.y, _path[i].z);
                var nextCorner = new Vector3(_path[i + 1].x, transform.position.y, _path[i + 1].z);
                Gizmos.DrawSphere(nextCorner, 0.5f);
                Gizmos.DrawLine(
                    curCorner,
                    nextCorner);
            }
        }
    }

    private void DoMovement()
    {
        _tankMover.ForceLevel();
        DoPathNavigation();
    }

    private void DoPathNavigation()
    {
        var nextPosition = _path.FirstOrDefault();
        if (nextPosition != Vector3.zero)
        {
            if (_facingPosition != nextPosition)
            {
                _tankMover.LookAt(nextPosition, true, out var _isFacingNextPosition);
                if (_isFacingNextPosition)
                {
                    Debug.Log("Finished looking at!");
                    _facingPosition = nextPosition;
                }
            }
            else
            {
                _tankMover.MoveTo(nextPosition, true, out var _isAtNextPosition);
                if (_isAtNextPosition)
                {
                    _facingPosition = null;
                    _path.RemoveAt(0);
                    if (_path.Count == 0)
                    {
                        Debug.Log("Finished navigation!");
                    }
                }
            }
        }
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        var navMeshPath = new NavMeshPath();
        if(_navMeshAgent.CalculatePath(targetPosition, navMeshPath))
        {
            _tankMover.StopRotate();
            _tankMover.StopMove();
            _path.Clear();
            foreach (var curCorner in navMeshPath.corners)
            {
                var distanceToCorner = Vector3.Distance(transform.position, curCorner);
                if (distanceToCorner > 1f)
                {
                    _path.Add(curCorner);
                }
            }

            _facingPosition = null;
        }
    }

    public void PositionOnNavMesh()
    {
        if (NavMesh.SamplePosition(
            transform.position,
            out var closestHit,
            500f,
            NavMesh.AllAreas))
        {
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            transform.position = closestHit.position;
        }
    }
}
