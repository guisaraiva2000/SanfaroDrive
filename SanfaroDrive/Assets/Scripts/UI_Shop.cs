using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using VehicleBehaviour;

public class UI_Shop : MonoBehaviour
{

    [SerializeField] private GameObject car;
    [SerializeField] Material[] carColors;
    [SerializeField] private Transform[] carParts;
    [SerializeField] private Transform[] colors;
    [SerializeField] private Transform[] upgrades;

    [SerializeField] private Material[] carMaterials;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject placeholderUI;

    private Material selectedMaterial;

    public GameObject Car
    {
        get => car;
        set => car = value;
    }

    private int gasCounter = 0;
    private int boostCounter = 0;
    private int jumpCounter = 0;

    public void Start()
    {

        selectedMaterial = null;

        foreach (Transform transform in carParts)
        {
            var bcolors = transform.GetComponentInChildren<Button>().colors;
            bcolors.selectedColor = new Color32(255, 153, 51, 255);
            bcolors.highlightedColor = new Color32(255, 153, 51, 100);
            transform.GetComponentInChildren<Button>().colors = bcolors;
            transform.GetComponentInChildren<Button>().onClick.AddListener(delegate { SelectItem(transform.Find("itemName").GetComponent<TextMeshProUGUI>().text); });
        }

        foreach (Transform transform in colors)
        {
            var bcolors = transform.GetComponent<Button>().colors;
            var normalColor = bcolors.normalColor;
            bcolors.selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.3f);
            bcolors.highlightedColor = bcolors.selectedColor;
            transform.GetComponent<Button>().colors = bcolors;
            transform.GetComponent<Button>().onClick.AddListener(delegate { BuyItem(transform.Find("itemName").GetComponent<TextMeshProUGUI>().text); });
        }

        foreach (Transform transform in upgrades)
        {
            transform.GetComponentInChildren<Button>().onClick.AddListener(delegate { UpgradeItem(transform.Find("itemName").GetComponent<TextMeshProUGUI>().text); });
        }

        Hide();
    }

    public void UpgradeItem(string name)
    {
        if (gameManager.GetComponent<MoneyManager>().Money >= 200)
        {
            foreach (Transform upgrade in upgrades)
            {
                if (upgrade.Find("itemName").GetComponent<TextMeshProUGUI>().text.Equals(name))
                {
                    if (name.Equals("Gas") && gasCounter < 3)
                    {
                        gasCounter++;
                        upgrade.Find("Image" + gasCounter).GetComponent<Image>().color = new Color32(255, 153, 51, 255);
                    }
                    else if (name.Equals("Boost") && boostCounter < 3)
                    {
                        boostCounter++;
                        upgrade.Find("Image" + boostCounter).GetComponent<Image>().color = new Color32(255, 153, 51, 255);
                    }
                    else if (name.Equals("Jump") && jumpCounter < 3)
                    {
                        jumpCounter++;
                        upgrade.Find("Image" + jumpCounter).GetComponent<Image>().color = new Color32(255, 153, 51, 255);
                    }
                }
            }
            gameManager.GetComponent<MoneyManager>().removeMoney(200);
            car.transform.Find("UI").GetComponent<CarCanvasController>().removeMoney(200);
            updateUpgrades();

            GameObject.Find("GameManager").GetComponent<AnalyticsController>().MoneySpentOnWorkshop(100);
        }
    }

    public void updateUpgrades()
    {
        if (gasCounter == 1)
        {
            car.GetComponent<FuelSystem>().FuelConsumptionRate = 0.6f;

        } else if (gasCounter == 2)
        {
            car.GetComponent<FuelSystem>().FuelConsumptionRate = 0.5f;

        } else if (gasCounter == 3)
        {
            car.GetComponent<FuelSystem>().FuelConsumptionRate = 0.4f;

        }

        if (boostCounter == 1)
        {
            car.GetComponent<BoostSystem>().BoostRegen = 1.5f;
            car.GetComponent<BoostSystem>().BoostForce = 7500;

        }
        else if (boostCounter == 2)
        {
            car.GetComponent<BoostSystem>().BoostRegen = 2f;
            car.GetComponent<BoostSystem>().BoostForce = 10000;

        }
        else if (boostCounter == 3)
        {
            car.GetComponent<BoostSystem>().BoostRegen = 3f;
            car.GetComponent<BoostSystem>().BoostForce = 12500;

        }

        if (jumpCounter == 1)
        {
            car.GetComponent<CarController>().JumpVel = 2.5f;

        }
        else if (jumpCounter == 2)
        {
            car.GetComponent<CarController>().JumpVel = 3f;

        }
        else if (jumpCounter == 3)
        {
            car.GetComponent<CarController>().JumpVel = 4;

        }
    }


    public void BuyItem(string color)
    {
        if (gameManager.GetComponent<MoneyManager>().Money >= 50)
        {
            if (selectedMaterial != null)
            {
                if (color.Equals("Black"))
                {
                    selectedMaterial.color = carColors[0].color;

                }
                else if (color.Equals("Grey"))
                {
                    selectedMaterial.color = carColors[1].color;

                }
                else if (color.Equals("White"))
                {
                    selectedMaterial.color = carColors[2].color;

                }
                else if (color.Equals("Red"))
                {
                    selectedMaterial.color = carColors[3].color;

                }
                else if (color.Equals("Green"))
                {
                    selectedMaterial.color = carColors[4].color;

                }
                else if (color.Equals("Blue"))
                {
                    selectedMaterial.color = carColors[5].color;

                }
                else if (color.Equals("Pink"))
                {
                    selectedMaterial.color = carColors[6].color;

                }
                else if (color.Equals("Purple"))
                {
                    selectedMaterial.color = carColors[7].color;

                }
                else if (color.Equals("Brown"))
                {
                    selectedMaterial.color = carColors[8].color;

                }
                else if (color.Equals("Yellow"))
                {
                    selectedMaterial.color = carColors[9].color;

                }
                else if (color.Equals("LightBlue"))
                {
                    selectedMaterial.color = carColors[10].color;

                }
                else if (color.Equals("Orange"))
                {
                    selectedMaterial.color = carColors[11].color;

                }

                selectedMaterial = null;
                gameManager.GetComponent<MoneyManager>().removeMoney(50);
                car.transform.Find("UI").GetComponent<CarCanvasController>().removeMoney(50);
            }

            foreach (Transform transform in carParts)
            {
                transform.Find("itemImage").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void SelectItem(string name)
    {

        if (name.Equals("Body"))
        {
            selectedMaterial = carMaterials[0];

        } else if (name.Equals("Mud"))
        {
            selectedMaterial = carMaterials[1];

        } else if (name.Equals("Window"))
        {
            selectedMaterial = carMaterials[2];

        } else if (name.Equals("Tire"))
        {
            selectedMaterial = carMaterials[3];

        } else if (name.Equals("Rim"))
        {
            selectedMaterial = carMaterials[4];

        }

        foreach(Transform transform in carParts)
        {
            if (transform.Find("itemName").GetComponent<TextMeshProUGUI>().text.Equals(name))
            {
                transform.Find("itemImage").GetComponent<Image>().color = new Color32(255, 153, 51, 255);
            } else
            {
                transform.Find("itemImage").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void Show()
    {
        placeholderUI = car.transform.Find("UI").gameObject;
        placeholderUI.transform.Find("FuelUI").gameObject.SetActive(false);
        placeholderUI.transform.Find("Boost").gameObject.SetActive(false);
        placeholderUI.transform.Find("MiniMap").gameObject.SetActive(false);
        placeholderUI.transform.Find("TopRight").gameObject.SetActive(false);
        car.gameObject.SetActive(false);
        car.gameObject.SetActive(true);
        car.gameObject.GetComponent<CarController>().enabled = false;
        gameObject.SetActive(true);
        carParts[0].GetComponentInChildren<Button>().Select();
    }

    public void Hide()
    {
        foreach (Transform transform in carParts)
        {
            transform.Find("itemImage").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        selectedMaterial = null;
        EventSystem.current.SetSelectedGameObject(null);
        gameObject.SetActive(false);
    }

    public void ExitWorkshop()
    {
        car.transform.position = new Vector3(car.transform.position.x, car.transform.position.y + 1f, car.transform.position.z - 2f);
        Hide();
        placeholderUI.transform.Find("FuelUI").gameObject.SetActive(true);
        placeholderUI.transform.Find("Boost").gameObject.SetActive(true);
        placeholderUI.transform.Find("MiniMap").gameObject.SetActive(true);
        placeholderUI.transform.Find("TopRight").gameObject.SetActive(true);
        car.gameObject.GetComponent<CarController>().enabled = true;
    }
}
