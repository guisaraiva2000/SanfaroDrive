using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class TriggerHide : MonoBehaviour
{
    private new Renderer renderer;
    private bool hidden;
    private GameObject car;
    private Passenger passenger;
    private GameObject beam;

    private const float AlphaStart = 0.25f;
    private const float AlphaEnd = 0.75f;
    private const float Duration = 1.5f;
    

    public bool Hidden
    {
        get => hidden;
        set => hidden = value;
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
        float lerp = Mathf.PingPong(Time.time, Duration) / Duration;
        float albedo = Mathf.Lerp(AlphaStart, AlphaEnd, lerp);
        Color color = renderer.material.color;
        renderer.material.color = new Color(color.r, color.g, color.b, albedo);

        var speed = Math.Abs(car.GetComponent<CarController>().Speed);
        if (speed <= 5)
        {
            if (hidden)
            {
                passenger.gameObject.SetActive(true);
                passenger.NavMeshAgent.SetDestination(passenger.transform.position);
                passenger.Animator.SetBool("enterMafiaCar", false);
                hidden = false;
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        hidden = true;
    }

    void OnTriggerExit(Collider other)
    {
        hidden = false;
    }

    public void DisableBeam(){
        beam.SetActive(false);
    }
}