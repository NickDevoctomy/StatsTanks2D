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
            transform.position = closestHit.position;
        }
    }
}
