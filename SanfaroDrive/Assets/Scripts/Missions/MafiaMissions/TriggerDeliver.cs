using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class TriggerDeliver : MonoBehaviour
{
    private new Renderer renderer;
    private bool delivered;
    private bool pickup;
    private bool pickedUp;
    private GameObject car;
    private Passenger passenger;
    private GameObject beam;

    private const float AlphaStart = 0.25f;
    private const float AlphaEnd = 0.75f;
    private const float Duration = 1.5f;
    
    [SerializeField] private GameObject destination;

    public bool Delivered
    {
        get => delivered;
        set => delivered = value;
    }

    public bool PickedUp
    {
        get => pickedUp;
        set => pickedUp = value;
    }
    
    public bool Pickup
    {
        get => pickup;
        set => pickup = value;
    }
    private void Start()
    {   
        beam = GameObject.Find("World").transform.Find("Beam").gameObject;
        beam.transform.position = transform.position;
        beam.SetActive(true);
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.green;
        car = gameObject.GetComponentInParent<MafiaMission>().Car;
        passenger = gameObject.GetComponentInParent<MafiaMission>().Passenger;
    }

    private void Update()
    {
        beam.transform.position = transform.position;
        float lerp = Mathf.PingPong(Time.time, Duration) / Duration;
        float albedo = Mathf.Lerp(AlphaStart, AlphaEnd, lerp);
        Color color = renderer.material.color;
        renderer.material.color = new Color(color.r, color.g, color.b, albedo);
        
        if (pickup)
        {
            passenger.transform.LookAt(car.transform);
        }
        
        var speed = Math.Abs(car.GetComponent<CarController>().Speed);
        if (speed <= 5)
        {
            if (delivered)
            {
                if (!pickup) // delivers and then hides the car
                {
                    destination.gameObject.SetActive(true);
                    passenger.transform.position = car.transform.position + car.transform.right;
                    passenger.gameObject.SetActive(true);
                    passenger.transform.LookAt(destination.transform);
                    passenger.Animator.SetBool("enterMafiaCar", true);
                    passenger.NavMeshAgent.SetDestination(destination.transform.position);
                    delivered = false;
                    pickup = true;
                    gameObject.SetActive(false);
                    
                }
                else // it already hid the car and come back to pick the robbers
                {
                    beam.transform.position = transform.position;
                    passenger.gameObject.SetActive(true);
                    
                    var distance = Vector3.Distance(passenger.transform.position, car.transform.position);
                    if (distance > 2)
                    {
                        passenger.Animator.SetBool("enterMafiaCar", true);
                        passenger.NavMeshAgent.SetDestination(car.transform.position);
                    }
                    else
                    {
                        delivered = false;
                        pickup = false;
                        pickedUp = true;
                        gameObject.SetActive(false);
                        passenger.gameObject.SetActive(false);
                        DisableBeam();
                    } 
                }
            } else if (passenger.Animator && passenger.Animator.GetBool("enterMafiaCar"))// passenger does not follow car
            {
                passenger.Animator.SetBool("enterMafiaCar", false);
                passenger.NavMeshAgent.SetDestination(passenger.transform.position);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            delivered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            delivered = false;
        }
    }

    public void DisableBeam(){
        beam.SetActive(false);
    }
}