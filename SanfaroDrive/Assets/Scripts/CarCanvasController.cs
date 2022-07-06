using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject moneyUI;
    [SerializeField] private GameObject moneyTMP;
    [SerializeField] private GameObject topLeftAddMoney;
    [SerializeField] private GameObject topLeftRemoveMoney;
    [SerializeField] private GameObject moneyChangeTMP;
    [SerializeField] private GameObject missionDistanceTMP;
    [SerializeField] private GameObject topRightBackMission;
    [SerializeField] private GameObject rewardTMP;
    [SerializeField] private GameObject distanceTMP;
    [SerializeField] private GameObject missionDistanceCoiso;
    [SerializeField] private Image icon;

    private bool popUpOn;
    private float popUpTime = 5f;

    // Update is called once per frame
    void Update()
    {
        moneyUI.SetActive(true);
        moneyTMP.GetComponent<TextMeshProUGUI>().text = gameManager.GetComponentInParent<MoneyManager>().Money + "$";
        if (popUpOn){
            popUpTime -= 1 * Time.deltaTime;
            
            if (popUpTime <= 0f){
                popUpTime = 5f;
                popUpOn = false;
                topLeftAddMoney.SetActive(false);
                topLeftRemoveMoney.SetActive(false);
                moneyChangeTMP.SetActive(false);
            }
        }
    }

    public void addMoney(int reward){
        popUpOn = true;
        topLeftAddMoney.SetActive(true);
        moneyChangeTMP.GetComponent<TextMeshProUGUI>().text = "+" + reward + "$";
        moneyChangeTMP.SetActive(true);
    }

    public void removeMoney(int cost){
        popUpOn = true;
        topLeftRemoveMoney.SetActive(true);
        moneyChangeTMP.GetComponent<TextMeshProUGUI>().text = "-" + cost + "$";
        moneyChangeTMP.SetActive(true);
    }

    public void displayNearest(Sprite missionIcon, int reward, int distance, bool onMission){
        if(onMission){
            missionDistanceCoiso.SetActive(true);
            missionDistanceTMP.SetActive(true);
            topRightBackMission.SetActive(true);
            missionDistanceTMP.GetComponent<TextMeshProUGUI>().text = (distance/100000.0).ToString("F1") + "\nkm";
        } 
        else{    
            icon.sprite = missionIcon;
            missionDistanceCoiso.SetActive(false);
            missionDistanceTMP.SetActive(false);
            topRightBackMission.SetActive(false);
            rewardTMP.GetComponent<TextMeshProUGUI>().text = reward + "$";
            distanceTMP.GetComponent<TextMeshProUGUI>().text = (distance/100000.0).ToString("F1") + "km";
        }


    }
}
