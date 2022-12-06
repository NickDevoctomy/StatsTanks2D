using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Vector3 _lastPosition;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!_navMeshAgent.pathPending &&
            Vector3.Distance(transform.position, _lastPosition) < 0.5f)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    _lastPosition = transform.position;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_navMeshAgent?.path != null)
        {
            for (int i = 0; i < _navMeshAgent.path.corners.Length - 1; i++)
            {
                Gizmos.color = Vector3.Distance(transform.position, _navMeshAgent.path.corners[i + 1]) > 5f ? Color.red : Color.green;
                Gizmos.DrawSphere(_navMeshAgent.path.corners[i + 1], 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    _navMeshAgent.path.corners[i],
                    _navMeshAgent.path.corners[i + 1]);
            }
        }

    }

    public void CalculatePath(Vector3 targetPosition)
    {
        _navMeshAgent.SetDestination(targetPosition);
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
            _lastPosition = transform.position;
        }
    }
}
