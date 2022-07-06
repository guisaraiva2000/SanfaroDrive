using System;
using TMPro;
using System.Linq;
using UnityEngine;
using VehicleBehaviour;


public class TaxiMissionController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject arrow;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private GameObject missionBack;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip timerSound;
    [SerializeField] private AudioClip triggerSound;
    [SerializeField] private AudioClip missionCompletedSound;
    [SerializeField] private AudioClip missionFailedSound;

    private TaxiMission nearestMission;
    private CarCanvasController canvasController;
    private TaxiMission currentMission;
    public bool onMission;
    private bool hasFailed;
    private float missionTimer;
    private float missionExpectedTime;
    private float failTimer;
    private Passenger dupNpc;
    private float missionDistance;
    private AudioSource audioSource;
    private float startFuel;

    private AnalyticsController analyticsController;

    public GameObject Car => car;

    private void Start()
    {
        GameObject canvas = car.GetComponent<CarController>().Canvas;
        canvasController = canvas.GetComponent<CarCanvasController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
        analyticsController = GameObject.Find("GameManager").GetComponent<AnalyticsController>();
        SetupMissions();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!onMission)
        {
            TaxiMission[] taxiMissions = FindNearestMissions(); // find 4 nearest missions
            nearestMission = taxiMissions[0]; 
            missionDistance = GetNearestDistance(nearestMission);
            canvasController.displayNearest(nearestMission.transform.Find("triggerStart").Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite, nearestMission.Reward, (int) missionDistance, false);
            EnableMissions(taxiMissions, true);

            foreach (TaxiMission taxiMission in taxiMissions)
            {
                if (taxiMission.TriggerStart.Started) // mission starts
                {
                    audioSource.clip = triggerSound; audioSource.Play();
                    StartMission(taxiMission);
                    EnableMissions(taxiMissions, false);
                    break;
                }
            }
            EnableMission(taxiMissions[3], false); // disables the 4th nearest
        }
        else
        {
            missionDistance = (currentMission.TriggerFinish.transform.position - car.transform.position).sqrMagnitude;
            canvasController.displayNearest(currentMission.transform.Find("triggerStart").Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite, currentMission.Reward, (int) missionDistance, true);
            missionTimer -= 1 * Time.deltaTime;
            CheckAudioTimer();
            
            if (missionTimer <= 0) // TODO mission failed text
            {
                audioSource.clip = missionFailedSound; audioSource.Play();
                FailureHandler();
                EndMission();
            }
            else
            {
                missionText.text = "Time Left: " + (int)missionTimer;
            
                var speed = Math.Abs(car.GetComponent<CarController>().Speed);
                if (speed <= 5 && currentMission.TriggerFinish.Exit)  // drops passenger at final stop
                {
                    audioSource.clip = missionCompletedSound; audioSource.Play();
                    GiveReward();
                    EndMission();
                }
            }
        }

        if (hasFailed) // TODO mission failed text disappears here
        {
            failTimer -= 1 * Time.deltaTime;
            if (failTimer <= 0f)
            {
                hasFailed = false;
                dupNpc.gameObject.SetActive(false);
            }
        }
    }

    private int GetNearestDistance(TaxiMission mission){
        return (int) (mission.TriggerStart.transform.position - mission.TriggerFinish.transform.position).sqrMagnitude;
    }

    private void SetupMissions()
    {
        foreach (Transform mission in transform)
        {
            TaxiMission taxiMission = mission.transform.GetComponent<TaxiMission>();
            taxiMission.Passenger.transform.position = taxiMission.TriggerStart.transform.position;
        }
    }

    private void EnableMissions(TaxiMission[] taxiMissions, bool activate)
    {
        foreach (TaxiMission taxiMission in taxiMissions)
        {
            EnableMission(taxiMission, activate);
        }
    }
    
    private void EnableMission(TaxiMission taxiMission, bool activate)
    {
        taxiMission.TriggerStart.gameObject.SetActive(activate);
        taxiMission.Passenger.gameObject.SetActive(activate);
    }
    
    private TaxiMission[] FindNearestMissions()
    {
        return transform.GetComponentsInChildren<TaxiMission>().OrderBy(mission => (
                mission.TriggerStart.transform.position - car.transform.position).sqrMagnitude
            ).Take(4).ToArray();
    }

    private void GiveReward()
    {
        int reputation = car.GetComponent<ReputationManager>().Reputation;
        GameObject gameManager = GameObject.Find("GameManager");
        int money = (currentMission.Reward) + (int)((reputation/100.0) * (int)missionTimer) ;
        canvasController.addMoney(money);
        gameManager.GetComponent<MoneyManager>().addMoney(money);
        car.GetComponent<ReputationManager>().addReputation();
    }
    
    private void StartMission(TaxiMission taxiMission)
    {
        // Analytics
        analyticsController.TotalTaxiMissions();
        startFuel = car.GetComponent<FuelSystem>().Fuel;
        
        taxiMission.TriggerFinish.gameObject.SetActive(true);
        taxiMission.TriggerStart.Started = false;
        missionTimer = taxiMission.Time;
        missionText.gameObject.SetActive(true);
        missionBack.SetActive(true);
        arrow.SetActive(true);
        arrow.GetComponent<DirectionalArrow>().Target = taxiMission.TriggerFinish.transform;
        currentMission = taxiMission;
        onMission = true;
        GameObject.Find("GameManager").GetComponent<MissionController>().OnMission = true;
    }

    private void EndMission()
    {
        // Analytics
        analyticsController.CompletedTaxiMissions();
        analyticsController.ExpectedTimeTaxiMission(currentMission.Time);
        analyticsController.CompletionTimeTaxiMission(currentMission.Time - missionTimer);
        analyticsController.TotalFuelPerMission(car.GetComponent<FuelSystem>().Fuel - startFuel);
        analyticsController.TotalTimeSpentTaxiMission(currentMission.Time - missionTimer);
        
        GetComponent<AudioSource>().Stop();
        currentMission.Passenger.transform.position = currentMission.TriggerStart.transform.position;
        currentMission.Passenger.gameObject.SetActive(false);
        arrow.SetActive(false);
        missionText.gameObject.SetActive(false);
        missionBack.SetActive(false);
        currentMission = null;
        onMission = false;
        GameObject.Find("GameManager").GetComponent<MissionController>().OnMission = false;
    }

    private void FailureHandler()
    {
        analyticsController.TotalTimeSpentTaxiMission(currentMission.Time - missionTimer);
        
        car.GetComponent<CarController>().Rb.velocity = Vector3.zero;
        dupNpc = Instantiate(currentMission.Passenger.gameObject.GetComponent<Passenger>());
        dupNpc.transform.position = car.transform.position + 4 * car.transform.right;
        dupNpc.gameObject.SetActive(true);
        dupNpc.transform.LookAt(car.transform);
        dupNpc.GetComponent<Animator>().SetBool("failed", true);
        car.GetComponent<ReputationManager>().removeReputation();
        hasFailed = true;
        failTimer = 10f;
        currentMission.TriggerFinish.DisableBeam();
        currentMission.TriggerFinish.gameObject.SetActive(false);
    }
    
    private void CheckAudioTimer()
    {
        if (missionTimer <= 10 && !GetComponent<AudioSource>().isPlaying)
        {
            audioSource.clip = timerSound;
            audioSource.Play();
        }
    }

}