using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private Transform path;
    [SerializeField] private int currentNode;
    private NavMeshAgent navMeshAgent;


    private List<Transform> nodes;
    private void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        

    }

    void FixedUpdate()
    {
        ApplySteer();
        CheckWaypointDistance();
    }

    private void ApplySteer()
    {

        navMeshAgent.SetDestination(nodes[currentNode].position);

    }
    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;

            }
        }
    }

}
