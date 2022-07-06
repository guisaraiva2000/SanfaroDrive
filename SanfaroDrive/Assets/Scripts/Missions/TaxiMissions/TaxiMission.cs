using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaxiMission : MonoBehaviour
{

    [Header("Input")] 
    [SerializeField] private TriggerStart triggerStart;
    [SerializeField] private TriggerFinish triggerFinish;
    [SerializeField] private Passenger passenger;
    [SerializeField] private float time = 120f;
    [SerializeField] private int reward = 5;
    
    private GameObject car;
    float distance;
    
    public TriggerStart TriggerStart
    {
        get => triggerStart;
        set => triggerStart = value;
    }

    public TriggerFinish TriggerFinish
    {
        get => triggerFinish;
        set => triggerFinish = value;
    }

    public Passenger Passenger
    {
        get => passenger;
        set => passenger = value;
    }

    public float Time
    {
        get => time;
        set => time = value;
    }

    public int Reward
    {
        get => reward;
        set => reward = value;
    }

    public float Distance
    {
        get => distance;
        set => distance = value;
    }

    public GameObject Car
    {
        get => car;
        set => car = value;
    }

    private void Start()
    {
        car = gameObject.GetComponentInParent<TaxiMissionController>().Car;
    }
}
