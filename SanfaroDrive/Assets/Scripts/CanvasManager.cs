using UnityEngine;
using TMPro;
using VehicleBehaviour;

public class CanvasManager : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI missionButton;
    private MissionController missionController;

    private GameObject selectedCar;
    private GameObject popUp;
    private Transform moneyUI;
    private GameObject placeholderCanvas;

    void Start(){
        missionController = GetComponentInParent<MissionController>();
    }

    public void RefuelButton(bool enable){
        if(enable){
            selectedCar = missionController.SelectedCar;
            placeholderCanvas = selectedCar.GetComponent<CarController>().Canvas;
            popUp = placeholderCanvas.transform.Find("LeftPopUp").Find("FuelPopUp").gameObject;
            popUp.SetActive(true);
        }
        else{
            popUp.SetActive(false);

        }
    }

    public void SwapCarButton(bool enable){
        if (enable)
        {
            selectedCar = missionController.SelectedCar;
            placeholderCanvas = selectedCar.GetComponent<CarController>().Canvas;
            popUp = placeholderCanvas.transform.Find("LeftPopUp").Find("SwitchCarPopUp").gameObject;
            popUp.SetActive(true);
        }
        else
        {
            if (popUp)
            {
                popUp.SetActive(false);
            }
        }
    }

    public void mafiaPopUp(bool enable, string message){
        missionButton.text = enable ? message : "";
        missionButton.gameObject.SetActive(enable);
    }
}
