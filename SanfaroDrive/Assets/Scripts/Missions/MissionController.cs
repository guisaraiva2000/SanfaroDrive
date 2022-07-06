using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using VehicleBehaviour;


public class MissionController : MonoBehaviour
{
    private KeyCode missionKey = KeyCode.V;
    [SerializeField] private GameObject taxiMissions;
    [SerializeField] private GameObject mafiaMissions;
    [SerializeField] private GameObject cinemachineManager;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject garages;

    [Header("NPCs")] 
    [SerializeField] private GameObject humanNpcs;
    [SerializeField] private GameObject carNpcs;
    [SerializeField] private GameObject carParkNpcs;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip triggerSound;

    private GameObject taxiCar;
    private GameObject mafiaCar;
    private GameObject selectedCar;
    private GameObject unselectedCar;
    private List<Transform> listGarages;
    private AudioSource audioSource;

    private float nextActionTime;
    private AnalyticsController analyticsController;

    private List<Transform> humanNpcsList;
    private List<Transform> carNpcsList;
    private List<Transform> carParkNpcsList;

    public bool OnMission { get; set; }
    
    public GameObject SelectedCar {  
        get => selectedCar;
        set => selectedCar = value; 
    }

    private void Start()
    {
        taxiCar = taxiMissions.GetComponent<TaxiMissionController>().Car;
        selectedCar = taxiCar;
        mafiaCar = mafiaMissions.GetComponent<MafiaMissionController>().Car;
        mafiaCar.GetComponent<CarController>().enabled = false;
        mafiaCar.GetComponent<CarController>().Canvas.SetActive(false);
        unselectedCar = mafiaCar;
        SetupGarages();
        unselectedCar.transform.position = FindNearestGaragePosition();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = triggerSound;
        //audioSource.volume = 0.5f;

        SetupNPCs();

        analyticsController = GetComponent<AnalyticsController>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(taxiCar.transform.position, mafiaCar.transform.position);
        if (distance < 10 && !OnMission)
        {
            GetComponent<CanvasManager>().SwapCarButton(true);
            if (Input.GetKeyDown(missionKey) || Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                audioSource.Play();
                bool isTaxiActive = taxiMissions.activeSelf;
                SwapCars(taxiCar, mafiaCar);
                EnableMissions(taxiMissions, taxiCar, !isTaxiActive);
                EnableMissions(mafiaMissions, mafiaCar, isTaxiActive);

                // Analytics
                analyticsController.TotalVehicleChanges();
            }
        }
        else
        {
            GetComponent<CanvasManager>().SwapCarButton(false);
        }
        
        UpdateGarage();
        UpdateNPCs();
        
        // Analytics every 1 sec heat map
        if (Time.time > nextActionTime) 
        {
            nextActionTime += 1.0f;
            analyticsController.HeatMap(selectedCar.transform.position);
        }
    }

    private void EnableMissions(GameObject missions, GameObject car, bool enable)
    {
        missions.SetActive(enable);
        CarController carController = car.GetComponent<CarController>();
        carController.enabled = enable;
        carController.Canvas.SetActive(enable);
        car.GetComponent<EngineSoundManager>().MuteSounds(!enable);
        car.transform.Find("Smoke").gameObject.SetActive(enable);

        if (enable)
        {
            cinemachineManager.GetComponent<CameraController>().Target = selectedCar = car;
            minimap.GetComponent<MiniMap>().car = car.transform;
        }
        else
        {
            unselectedCar = car;
        }
    }

    private static void SwapCars(GameObject car1, GameObject car2)
    {
        (car1.transform.position, car2.transform.position) = (car2.transform.position, car1.transform.position);
        (car1.transform.rotation, car2.transform.rotation) = (car2.transform.rotation, car1.transform.rotation);
        
        car1.SetActive(false); car1.SetActive(true);
        car2.SetActive(false); car2.SetActive(true);
    }

    private void UpdateGarage()
    {
        float carGarageDist = GetDistance(selectedCar.transform.position, FindNearestGaragePosition());
        float carCarDist = GetDistance(selectedCar.transform.position, unselectedCar.transform.position);
        
        if (carGarageDist < carCarDist && Math.Abs(carGarageDist - carCarDist) > 1)
        {
            unselectedCar.transform.position = FindNearestGaragePosition();
        }
    }

    private void UpdateNPCs()
    {
        ActivateNPCs(humanNpcsList);
        ActivateNPCs(carNpcsList);
        ActivateNPCs(carParkNpcsList);
    }
    
    private void ActivateNPCs(List<Transform> npcList)
    {
        foreach (Transform npc in npcList)
        {
            if (GetDistance(selectedCar.transform.position, npc.transform.position) <= 150)
            {
                npc.gameObject.SetActive(true);
            }
            else if (npc.gameObject.activeSelf)
            {
                npc.gameObject.SetActive(false);
            }
        }
    }
    
    private Vector3 FindNearestGaragePosition()
    {
        return listGarages.OrderBy(garage => 
            GetDistance(garage.transform.position, selectedCar.transform.position)
        ).First().position;
    }

    private float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2);
    }
    
    public void EnableCarCanvas(bool enable)
    {
        selectedCar.GetComponent<CarController>().Canvas.SetActive(enable);
    }
    
    private void SetupGarages()
    {
        listGarages = new List<Transform>();
        foreach (Transform garage in garages.transform)
        {
            listGarages.Add(garage);
        }
    }

    private void SetupNPCs()
    {
        humanNpcsList = SetupNPCsList(humanNpcs);
        carNpcsList = SetupNPCsList(carNpcs);
        carParkNpcsList = SetupNPCsList(carParkNpcs);
    }

    private List<Transform> SetupNPCsList(GameObject npcGameObject)
    {
        List<Transform> npcList = new List<Transform>();
        foreach (Transform npcs in npcGameObject.transform)
        {
            foreach (Transform npc in npcs.transform)
            {
                npcList.Add(npc);
                npc.gameObject.SetActive(false);
            }
        }

        return npcList;
    }
}
