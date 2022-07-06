using System.Linq;
using UnityEngine;
using VehicleBehaviour;

public class WaterTrigger : MonoBehaviour
{
    [Header("Reset Spots")]
    [SerializeField] private GameObject spots;

    void OnTriggerEnter(Collider other)
    {
        Transform spot = FindNearestResetSpot(other.transform.position);
        Transform car = other.transform.parent;
        car.position = spot.position;
        car.rotation = new Quaternion(0f, 0f, 0f, 0f);
        /*car.gameObject.SetActive(false); 
        car.gameObject.SetActive(true);*/
        if (car.gameObject.GetComponent<CarController>().Rb)
        {
            car.gameObject.GetComponent<CarController>().Rb.velocity = Vector3.zero;
        }
    }
    
    private Transform FindNearestResetSpot(Vector3 carPosition)
    {
        Transform[] spotsList = spots.GetComponentsInChildren<Transform>().Where(t => t != spots.transform).ToArray();
        return spotsList.OrderBy(spot => (
            spot.position - carPosition).sqrMagnitude
        ).First();
    }
}