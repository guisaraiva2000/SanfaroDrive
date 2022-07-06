using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelProgress : MonoBehaviour
{
    public Image loadingImage;
    [SerializeField] public Image[] iconImage;
    public Image currentIcon;
    //public Text loadingText;
    [Range(0, 1)]
    public float loadingProgress = 0;
    // Update is called once per frame
    public void Update()
    {
        loadingImage.fillAmount = GetComponent<FuelSystem>().Fuel / 100;
        if(GetComponent<FuelSystem>().Fuel > 75){
            iconImage[0].enabled = true;
            iconImage[1].enabled = false;
            iconImage[2].enabled = false;
        }
        else if(GetComponent<FuelSystem>().Fuel > 30){
            iconImage[1].enabled = true;
            iconImage[0].enabled = false;
            iconImage[2].enabled = false;
        }
        else if(GetComponent<FuelSystem>().Fuel >= 0){
            iconImage[2].enabled = true;
            iconImage[1].enabled = false;
            iconImage[0].enabled = false;
        }
        /*if(loadingProgress < 1)
        {
            loadingText.text = Mathf.RoundToInt(GetComponent<FuelSystem>().startfuel) + "%\nLoading...";
        }
        else
        {
            loadingText.text = "Done.";
        }*/
    }
}
