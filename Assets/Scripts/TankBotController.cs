using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
        var nextPosition = _path.FirstOrDefault();

        // check angle to next position
        if(nextPosition != Vector3.zero &&
            _facingPosition != nextPosition)
        {
            _tankMover.LookAt(nextPosition, true, out var _isFacingNextPosition);
            if (_isFacingNextPosition)
            {
                _facingPosition = nextPosition;
            }
        }

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
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        var navMeshPath = new NavMeshPath();
        _navMeshAgent.CalculatePath(targetPosition, navMeshPath);
        _path.Clear();
        foreach(var curCorner in navMeshPath.corners)
        {
            var distanceToCorner = Vector3.Distance(transform.position, curCorner);
            if(distanceToCorner > 1f)
            {
                _path.Add(curCorner);
            }
        }

        _facingPosition = null;
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

    private float GetAngleToNext(Vector3 nextCorner)
    {
        var forwardVector = transform.position + transform.forward * 2;
        var side1 = forwardVector - transform.position;
        var side2 = nextCorner - transform.position;
        return Vector3.Angle(transform.position, nextCorner);
    }
}
