using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class TriggerStart : MonoBehaviour
{
    private new Renderer renderer;
    private bool enter;
    private bool started;
    private GameObject car;
    private Passenger passenger;
    private string enterAnimation;
    

    public bool Started
    {
        get => started;
        set => started = value;
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        if (transform.parent.parent.name.Equals("TaxiMissions"))
        {
            car = gameObject.GetComponentInParent<TaxiMission>().Car;
            passenger = gameObject.GetComponentInParent<TaxiMission>().Passenger;
            enterAnimation = "enterTaxi";
        }
        else
        {
            car = gameObject.GetComponentInParent<MafiaMission>().Car;
            passenger = gameObject.GetComponentInParent<MafiaMission>().Passenger;
            enterAnimation = "enterMafiaCar";

        }
    }

    private void Update()
    {
        passenger.transform.LookAt(car.transform);

        var speed = Math.Abs(car.GetComponent<CarController>().Speed);
        var distance = Vector3.Distance(passenger.transform.position, car.transform.position);
        if (distance > 2 && speed <= 5 && enter) // passenger enters car
        {
            passenger.Animator.SetBool(enterAnimation, true);
            passenger.NavMeshAgent.SetDestination(car.transform.position);
        }
        else if (!enter && passenger.Animator && passenger.Animator.GetBool(enterAnimation)) // passenger does not follow car
        {
            passenger.Animator.SetBool(enterAnimation, false);
            passenger.NavMeshAgent.SetDestination(passenger.transform.position);
        }
        else if (distance <= 2 && speed <= 5 && enter) // mission starts
        {
            enter = false;
            started = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            enter = true;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            enter = false;
        }
    }
}