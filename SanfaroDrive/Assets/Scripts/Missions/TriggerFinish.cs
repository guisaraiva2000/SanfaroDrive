using System;
using UnityEngine;
using UnityEngine.AI;
using VehicleBehaviour;


public class TriggerFinish : MonoBehaviour
{
    private new Renderer renderer;
    private GameObject car;
    private Passenger passenger;
    private bool exit;
    private bool hasFinished;
    private Passenger dupNpc;
    private float currentTime = 10f;
    private Vector3 dropPosition;
    private GameObject beam;
    
    private const float AlphaStart = 0.25f;
    private const float AlphaEnd = 0.75f;
    private const float Duration = 1.5f;
    

    public bool Exit
    {
        get => exit;
        set => exit = value;
    }
    
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.green;
        dropPosition = transform.position + Vector3.back * 5;
        beam = GameObject.Find("World").transform.Find("Beam").gameObject;
        beam.transform.position = transform.position;
        beam.SetActive(true);
        if (transform.parent.parent.name.Equals("TaxiMissions"))
        {
            car = gameObject.GetComponentInParent<TaxiMission>().Car;
            passenger = gameObject.GetComponentInParent<TaxiMission>().Passenger;
        }
        else
        {
            car = gameObject.GetComponentInParent<MafiaMission>().Car;
            passenger = gameObject.GetComponentInParent<MafiaMission>().Passenger;
        } 
        dupNpc = Instantiate(passenger.gameObject.GetComponent<Passenger>()); 
        dupNpc.gameObject.SetActive(false);
    }

    private void Update()
    {
        float lerp = Mathf.PingPong(Time.time, Duration) / Duration;
        float albedo = Mathf.Lerp(AlphaStart, AlphaEnd, lerp);
        Color color = renderer.material.color;
        renderer.material.color = new Color(color.r, color.g, color.b, albedo);
        
        var speed = Math.Abs(car.GetComponent<CarController>().Speed);
        if (speed <= 5)
        {
            if (exit && !hasFinished)
            {
                dupNpc.transform.position = car.transform.position + car.transform.right;
                dupNpc.gameObject.SetActive(true);
                dupNpc.transform.LookAt(dropPosition);
                dupNpc.GetComponent<Animator>().SetBool("enterTaxi", true);
                dupNpc.GetComponent<NavMeshAgent>().SetDestination(dropPosition);
                exit = false;
                hasFinished = true;
                renderer.enabled = false;
            }
        }

        if (hasFinished)
        {
            DisableBeam();
            var distance = Vector3.Distance(dropPosition, dupNpc.transform.position);
            if (distance <= 1)
            {
                currentTime -= 1 * Time.deltaTime;
                dupNpc.Animator.SetBool("exitTaxi", true);
                dupNpc.transform.LookAt(car.transform);

                if (currentTime <= 0)
                {
                    currentTime = 10f; 
                    hasFinished = false;
                    dupNpc.GetComponent<Animator>().SetBool("enterTaxi", false);
                    dupNpc.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    renderer.enabled = true;
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            exit = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            exit = false;
        }
    }

    public void DisableBeam()
    {
        if (beam && beam.activeSelf)
        {
            beam.SetActive(false);
        }
    }
}