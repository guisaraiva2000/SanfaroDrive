using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelProgress : MonoBehaviour
{
    [SerializeField] public Image lowFuel;
    [SerializeField] public Image[] frontRow;
    [SerializeField] public Image[] background;
    private Image currentFront;
    private float blinkCount = 0;
    // Update is called once per frame
    public void Update()
    {

        if (GetComponent<FuelSystem>().Fuel > 75)
        {
            currentFront = frontRow[0];
            background[0].enabled = true;
            background[1].enabled = false;
            background[2].enabled = false;
            frontRow[0].enabled = true;
            frontRow[1].enabled = false;
            frontRow[2].enabled = false;
            lowFuel.gameObject.SetActive(false);
        }
        else if (GetComponent<FuelSystem>().Fuel > 30)
        {
            currentFront = frontRow[1];
            background[1].enabled = true;
            background[0].enabled = false;
            background[2].enabled = false;
            frontRow[1].enabled = true;
            frontRow[0].enabled = false;
            frontRow[2].enabled = false;
        }
        else if (GetComponent<FuelSystem>().Fuel >= 0)
        {
            blinkCount += 1 * Time.deltaTime;
            if ((int)(blinkCount % 2) == 0)
            {
                lowFuel.gameObject.SetActive(true);
            }
            else
            {
                lowFuel.gameObject.SetActive(false);
            }
            currentFront = frontRow[2];
            background[2].enabled = true;
            background[1].enabled = false;
            background[0].enabled = false;
            frontRow[2].enabled = true;
            frontRow[1].enabled = false;
            frontRow[0].enabled = false;

        }
        currentFront.fillAmount = GetComponent<FuelSystem>().Fuel / 100;
    }

}
