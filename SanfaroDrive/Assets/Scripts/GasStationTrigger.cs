using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using VehicleBehaviour;


public class GasStationTrigger : MonoBehaviour
{
    private KeyCode gasKey = KeyCode.V;
    private new Renderer renderer;
    private GameObject car;
    private bool enter;
    private GameObject gameManager;
    private GameObject selectedCar;
    private CarCanvasController canvasController;
    private AudioSource audioSource;
    private bool hasPlayed;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip triggerSound;

    private const float AlphaStart = 0.25f;
    private const float AlphaEnd = 0.75f;
    private const float Duration = 1.5f;

    
    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = triggerSound;
        audioSource.playOnAwake = false;
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.blue;
    }

    private void Update()
    {
        selectedCar = gameManager.GetComponentInParent<MissionController>().SelectedCar;
        GameObject canvas = selectedCar.GetComponent<CarController>().Canvas;
        canvasController = canvas.GetComponent<CarCanvasController>();
        float lerp = Mathf.PingPong(Time.time, Duration) / Duration;
        float albedo = Mathf.Lerp(AlphaStart, AlphaEnd, lerp);
        Color color = renderer.material.color;
        renderer.material.color = new Color(color.r, color.g, color.b, albedo);

        if (car)
        {
            var speed = Math.Abs(car.GetComponent<CarController>().Speed);
            if (speed <= 15 && enter)
            {
                gameManager.GetComponent<CanvasManager>().RefuelButton(true);
                if (!audioSource.isPlaying && !hasPlayed)
                {
                    audioSource.Play();
                    hasPlayed = true;
                }
                if (Input.GetKeyDown(gasKey) || Input.GetKeyDown(KeyCode.Joystick1Button3))
                {
                    FuelSystem fuelSystem = car.GetComponent<FuelSystem>();
                    MoneyManager moneyManager = GameObject.Find("GameManager").GetComponent<MoneyManager>();
                    int moneyToSpend = 100 - (int)fuelSystem.Fuel;
                    if (moneyManager.Money - moneyToSpend >= 0)
                    {
                        fuelSystem.ReFuel(100);
                    }
                    else
                    {
                        moneyToSpend = moneyManager.Money;
                        fuelSystem.ReFuel(fuelSystem.Fuel + moneyToSpend);
                    }
                    canvasController.removeMoney(moneyToSpend);
                    moneyManager.removeMoney(moneyToSpend);
                    gameManager.GetComponent<AnalyticsController>().MoneySpentOnFuel(moneyToSpend);
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.name is "TaxiCar")
        {
            enter = true;
            car = other.transform.parent.gameObject;
        } 
        else if (other.transform.parent.name is "MafiaCar")
        {
            enter = true;
            car = other.transform.parent.gameObject;
        }

        hasPlayed = false;
    }
    void OnTriggerExit(Collider other)
    {
        gameManager.GetComponent<CanvasManager>().RefuelButton(false);
        if (other.transform.parent.name is "MafiaCar" or "TaxiCar")
        {
            enter = false;
        }
    }
}