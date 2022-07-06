using UnityEngine;
using UnityEngine.AI;

public class Passenger : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
}
