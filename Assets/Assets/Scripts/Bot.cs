using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Bot : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    //void Update()
    //{
    //}

    void OnDrawGizmos()
    {
        if(_navMeshAgent?.path != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _navMeshAgent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawSphere(_navMeshAgent.path.corners[i + 1], 0.5f);
                Gizmos.DrawLine(
                    _navMeshAgent.path.corners[i],
                    _navMeshAgent.path.corners[i + 1]);
            }
        }

    }

    public void CalculatePath(Vector3 targetPosition)
    {
        var navMeshPath = new NavMeshPath();
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
        }
    }
}
