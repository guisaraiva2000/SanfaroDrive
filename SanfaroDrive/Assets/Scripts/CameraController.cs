using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    [SerializeField] private CinemachineVirtualCamera mainMenuCamera;
    [SerializeField] private CinemachineVirtualCamera workshopCamera;
    [SerializeField] private KeyCode toggleCameras = KeyCode.Tab;


    private int currentCameraIndex;
    private bool onMainMenu;
    private bool workshop;
    private GameObject car;
    private GameObject target;

    public bool OnMainMenu
    {
        get => onMainMenu;
        set => onMainMenu = value;
    }

    public bool Workshop
    {
        get => workshop;
        set => workshop = value;
    }

    public GameObject Target
    {
        get => target;
        set => target = value;
    }

    public CinemachineVirtualCamera WorkshopCamera
    {
        get => workshopCamera;
        set => workshopCamera = value;
    }

    public GameObject Car
    {
        get => car;
        set => car = value;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(toggleCameras) || Input.GetKeyDown(KeyCode.Joystick1Button13)) && !onMainMenu)
        {
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        }
        SwitchCamera();
    }

    void SwitchCamera()
    {
        if (onMainMenu){
            foreach (var camera in cameras)
            {
                camera.Priority = 0;
            }
            mainMenuCamera.Priority = 1;
        }
        else if (workshop)
        {
            cameras[0].Priority = 0;
            cameras[1].Priority = 0;
            cameras[2].Priority = 0;
            workshopCamera.LookAt = car.transform;
            workshopCamera.Priority = 1;
        }
        else if(!onMainMenu){
            mainMenuCamera.Priority = 0;
            workshopCamera.Priority = 0;
            foreach (var camera in cameras)
            {
                if (camera == cameras[currentCameraIndex])
                {
                    camera.Priority = 1;
                    if (target)
                    {
                        camera.LookAt = camera.Follow = target.transform;
                    }
                    
                } 
                else
                {
                    camera.Priority = 0;
                }
            }
        }
    }

    public void setMainMenuCamera(){
        onMainMenu = true;
        SwitchCamera();
    }

    public void playCamera(){
        onMainMenu = false;
        SwitchCamera();
    }
}