using System;
using TMPro;
using System.Linq;
using UnityEngine;
using VehicleBehaviour;


public class MafiaMissionController : MonoBehaviour
{
    [Header("Input")]
    //[SerializeField] private KeyCode missionKey = KeyCode.E;
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject arrow;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private GameObject missionBack;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip timerSound;
    [SerializeField] private AudioClip triggerSound;
    [SerializeField] private AudioClip alarm;
    [SerializeField] private AudioClip missionCompletedSound;
    [SerializeField] private AudioClip missionFailedSound;

    private MafiaMission nearestMission;
    private MafiaMission currentMission;
    private bool onMission;
    private bool hasFailed;
    private float missionTimer;
    private float failTimer;
    private Passenger dupNpc;
    private CanvasManager canvasManager;
    private CarCanvasController canvasController;
    private float distanceToTrigger;
    private GameObject currentTrigger;
    private AudioSource audioSource;

    private float startFuel;
    
    private AnalyticsController analyticsController;

    public GameObject Car => car;
    
    private void Start()
    {
        GameObject canvas = car.GetComponent<CarController>().Canvas;
        canvasController = canvas.GetComponent<CarCanvasController>();
        GameObject gameManager = GameObject.Find("GameManager");
        canvasManager = gameManager.GetComponent<CanvasManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
        analyticsController = gameManager.GetComponent<AnalyticsController>();
        SetupMissions();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onMission)
        {
            MafiaMission[] mafiaMissions = FindNearestMissions(); // find 4 nearest missions
            nearestMission = mafiaMissions[0];
            distanceToTrigger = GetNearestDistance(nearestMission);
            canvasController.displayNearest(nearestMission.transform.Find("triggerStart").Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite, nearestMission.Reward, (int) distanceToTrigger, false);
            EnableMissions(mafiaMissions, true);

            foreach (MafiaMission mafiaMission in mafiaMissions)
            {
                if (mafiaMission.TriggerStart.Started) // mission starts
                {
                    audioSource.clip = triggerSound; audioSource.Play();
                    StartMission(mafiaMission);
                    EnableMissions(mafiaMissions, false);
                    TriggerDeliver triggerDeliver = currentMission.TriggerDeliver;
                    currentTrigger = triggerDeliver.transform.gameObject;
                    canvasManager.mafiaPopUp(true, "Drop the client at the destination");
                    break;
                }
            }
            EnableMission(mafiaMissions[3], false); // disables the 4th nearest
        }
        else
        {
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
            
                TriggerDeliver triggerDeliver = currentMission.TriggerDeliver;
                TriggerHide triggerHide = currentMission.TriggerHide;
                TriggerFinish triggerFinish = currentMission.TriggerFinish;
                distanceToTrigger = (currentTrigger.transform.position - car.transform.position).sqrMagnitude;
                canvasController.displayNearest(currentMission.transform.Find("triggerStart").Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite, currentMission.Reward, (int) distanceToTrigger, true);
                var speed = Math.Abs(car.GetComponent<CarController>().Speed);
                if (speed <= 5)
                {
                    if (triggerDeliver.Delivered && !triggerDeliver.Pickup) // delivers at criminal spot
                    {
                        audioSource.clip = triggerSound; audioSource.Play();
                        currentTrigger = triggerHide.gameObject;
                        canvasManager.mafiaPopUp(true, "Hide the car");
                        GiveReward();
                        triggerHide.gameObject.SetActive(true);
                        arrow.GetComponent<DirectionalArrow>().Target = triggerHide.transform;
                    }
                    else if (triggerDeliver.PickedUp) // pickup and runaway
                    {
                        audioSource.clip = triggerSound; audioSource.Play();
                        currentTrigger = triggerFinish.gameObject;
                        canvasManager.mafiaPopUp(true, "Get to the drop off site");
                        GiveReward();
                        triggerFinish.gameObject.SetActive(true);
                        arrow.GetComponent<DirectionalArrow>().Target = triggerFinish.transform;
                        triggerDeliver.PickedUp = false;
                    }

                    else if (triggerHide.Hidden) // hid the car
                    {
                        audioSource.clip = alarm; audioSource.Play();
                        currentTrigger = triggerDeliver.gameObject;
                        canvasManager.mafiaPopUp(true, "Pick your client up again");
                        GiveReward();
                        triggerDeliver.gameObject.SetActive(true);
                        arrow.GetComponent<DirectionalArrow>().Target = triggerDeliver.transform;
                    }
                    
                    else if (triggerFinish.Exit)  // drops passenger at final stop
                    {
                        audioSource.clip = missionCompletedSound; audioSource.Play();
                        canvasManager.mafiaPopUp(false, "");
                        GiveReward();
                        EndMission();
                    }
                }
            }
        }
        
        if (hasFailed) // TODO mission failed text disappears here
        {
            failTimer -= 1 * Time.deltaTime;
            if (failTimer <= 0f)
            {
                hasFailed = false;
                if (dupNpc && dupNpc.gameObject.activeSelf)
                {
                    dupNpc.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetupMissions()
    {
        foreach (Transform mission in transform)
        {
            MafiaMission mafiaMission = mission.transform.GetComponent<MafiaMission>();
            mafiaMission.Passenger.transform.position = mafiaMission.TriggerStart.transform.position;
        }
    }

    private void EnableMissions(MafiaMission[] mafiaMissions, bool activate)
    {
        foreach (MafiaMission mafiaMission in mafiaMissions)
        {
            EnableMission(mafiaMission, activate);
        }
    }

    private void EnableMission(MafiaMission mafiaMission, bool activate)
    {
        mafiaMission.TriggerStart.gameObject.SetActive(activate);
        mafiaMission.Passenger.gameObject.SetActive(activate);
    }

    private MafiaMission[] FindNearestMissions()
    {
        return transform.GetComponentsInChildren<MafiaMission>().OrderBy(mission => (
            mission.TriggerStart.transform.position - car.transform.position).sqrMagnitude
        ).Take(4).ToArray();
    }
    
    private void GiveReward()
    {
       int reputation = car.GetComponent<ReputationManager>().Reputation;
       GameObject gameManager = GameObject.Find("GameManager");

       int money = (int)((currentMission.Reward / 4) * reputation / 100.0);
       canvasController.addMoney(money);
       gameManager.GetComponent<MoneyManager>().addMoney(money);
    }
    
    private void StartMission(MafiaMission mafiaMission)
    {
        analyticsController.TotalMafiaMission();
        startFuel = car.GetComponent<FuelSystem>().Fuel;
        
        mafiaMission.TriggerDeliver.gameObject.SetActive(true);
        mafiaMission.TriggerStart.Started = false;
        missionTimer = mafiaMission.Time;
        missionBack.SetActive(true);
        missionText.gameObject.SetActive(true);
        arrow.SetActive(true);
        arrow.GetComponent<DirectionalArrow>().Target = mafiaMission.TriggerDeliver.transform;
        currentMission = mafiaMission;
        onMission = true;
        GameObject.Find("GameManager").GetComponent<MissionController>().OnMission = true;
    }

    private int GetNearestDistance(MafiaMission mission){
        return (int) (mission.TriggerStart.transform.position - mission.TriggerFinish.transform.position).sqrMagnitude;
    }

    private void EndMission()
    {
        // Analytics
        analyticsController.CompletedMafiaMissions();
        analyticsController.ExpectedTimeMafiaMission(currentMission.Time);
        analyticsController.CompletionTimeMafiaMission(currentMission.Time - missionTimer);
        analyticsController.TotalFuelPerMission(car.GetComponent<FuelSystem>().Fuel - startFuel);
        analyticsController.TotalTimeSpentMafiaMission(currentMission.Time - missionTimer);

        currentMission.Passenger.transform.position = currentMission.TriggerStart.transform.position;
        currentMission.Passenger.gameObject.SetActive(false);
        arrow.SetActive(false);
        missionText.gameObject.SetActive(false);
        missionBack.SetActive(false);
        currentMission = null;
        onMission = false;
        car.GetComponent<ReputationManager>().addReputation();
        GameObject.Find("GameManager").GetComponent<MissionController>().OnMission = false;
    }

    private void FailureHandler()
    {
        audioSource.clip = triggerSound; audioSource.Play();
        analyticsController.TotalTimeSpentMafiaMission(currentMission.Time - missionTimer);
        
        car.gameObject.SetActive(false);
        car.gameObject.SetActive(true);
        if (currentTrigger != currentMission.TriggerHide.gameObject)
        {
            dupNpc = Instantiate(currentMission.Passenger.gameObject.GetComponent<Passenger>());
            dupNpc.transform.position = car.transform.position + 4 * car.transform.right;
            dupNpc.gameObject.SetActive(true);
            dupNpc.transform.LookAt(car.transform);
            dupNpc.GetComponent<Animator>().SetBool("failed", true);
        }
        car.GetComponent<ReputationManager>().removeReputation();
        hasFailed = true;
        failTimer = 10f;
        currentMission.TriggerFinish.DisableBeam();
        currentMission.TriggerDeliver.DisableBeam();
        currentMission.TriggerHide.DisableBeam();
        currentMission.TriggerFinish.gameObject.SetActive(false);
        currentMission.TriggerDeliver.gameObject.SetActive(false);
        currentMission.TriggerHide.gameObject.SetActive(false);
        canvasManager.mafiaPopUp(false, "This is not supposed to show up");
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