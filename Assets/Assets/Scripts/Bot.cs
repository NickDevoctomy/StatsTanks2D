using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    //void Start()
    //{
    //}

    //void Update()
    //{
    //}

    public void PositionOnNavMesh()
    {
        if (NavMesh.SamplePosition(
            transform.position,
            out var closestHit,
            500f,
            NavMesh.AllAreas))
        {
            var navMeshAgent = GetComponent<NavMeshAgent>();
            Debug.Log($"Positioning agent {navMeshAgent.agentTypeID}");
            transform.position = closestHit.position;
        }
    }
}
