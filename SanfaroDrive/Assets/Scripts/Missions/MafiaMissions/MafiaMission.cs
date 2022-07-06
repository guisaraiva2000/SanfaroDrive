using UnityEngine;

public class MafiaMission : MonoBehaviour
{

    [Header("Input")] 
    [SerializeField] TriggerStart triggerStart;
    [SerializeField] TriggerDeliver triggerDeliver;
    [SerializeField] TriggerHide triggerHide;
    [SerializeField] TriggerFinish triggerFinish;
    [SerializeField] GameObject passengerDestination;
    [SerializeField] Passenger passenger;
    [SerializeField] float time = 120f;
    [SerializeField] int reward = 5;
    
    private GameObject car;
    private float distance;


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

    public TriggerDeliver TriggerDeliver
    {
        get => triggerDeliver;
        set => triggerDeliver = value;
    }

    public TriggerHide TriggerHide
    {
        get => triggerHide;
        set => triggerHide = value;
    }

    public GameObject PassengerDestination
    {
        get => passengerDestination;
        set => passengerDestination = value;
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
        car = gameObject.GetComponentInParent<MafiaMissionController>().Car;
    }
}