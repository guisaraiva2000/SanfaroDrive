using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class ShopTriggerCollider : MonoBehaviour
{
    [SerializeField] private Transform cineMachineManager;
    // Start is called before the first frame update
    private bool hasEntered;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip triggerSound;
    [SerializeField] private UI_Shop uiShopTaxi;
    [SerializeField] private UI_Shop uiShopMafia;

    private AudioSource audioSource;
     private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!hasEntered)
        {
            audioSource.clip = triggerSound; audioSource.Play();
            GameObject.Find("GameManager").GetComponent<AnalyticsController>().WorkshopVisits();
            cineMachineManager.GetComponent<CameraController>().WorkshopCamera.transform.position = new Vector3(transform.position.x - 4f, transform.position.y + 3f, transform.position.z + 2f);
            cineMachineManager.GetComponent<CameraController>().Workshop = true;
            cineMachineManager.GetComponent<CameraController>().Car = collider.gameObject;
            if (collider.gameObject.transform.parent.name.Equals("TaxiCar"))
            {
                uiShopTaxi.Show();
            } else
            {
                uiShopMafia.Show();
            }
            hasEntered = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.transform.parent.name.Equals("TaxiCar"))
        {
            uiShopTaxi.Hide();
        }
        else
        {
            uiShopMafia.Hide();
        }
        hasEntered = false;
        cineMachineManager.GetComponent<CameraController>().Workshop = false;
    }
}
