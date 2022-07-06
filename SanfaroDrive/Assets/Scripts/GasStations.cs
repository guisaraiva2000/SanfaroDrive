using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GasStations : MonoBehaviour
{
    [SerializeField]
    private GameObject gasStations;

    List<Transform> listGasStations;
    private GameObject selectedCar;
    private MissionController missionController;

    private void Start()
    {
        listGasStations = new List<Transform>();
        foreach (Transform gasStation in gasStations.transform)
        {
            listGasStations.Add(gasStation);
        }

        missionController = GameObject.Find("GameManager").GetComponent<MissionController>();
    }


    void Update()
    {
        selectedCar = missionController.SelectedCar;
        Transform[] closestGasStations = FindNearestGasStations();
        closestGasStations[0].gameObject.SetActive(true);
        closestGasStations[1].gameObject.SetActive(false);
    }


    private Transform[] FindNearestGasStations()
    {
        return listGasStations.OrderBy(gasStation => (
                gasStation.transform.position - selectedCar.transform.position).sqrMagnitude
            ).Take(2).ToArray();
    }
}
