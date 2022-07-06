using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VehicleBehaviour;

public class BoostSystem : MonoBehaviour {

    public bool hasboost;
    public bool hasBoost { get => hasboost;}
	public float boostConsumptionRate; //boost drop rate
	public Image boostIndicator; //slider to indicate the boost level

	// Maximum boost available
	[SerializeField] float maxBoost = 10f;
	public float MaxBoost { get => maxBoost;
		set => maxBoost = value;
	}

	// Current boost available
	[SerializeField] float boost = 10f;
	public float Boost { get => boost;
		set => boost = Mathf.Clamp(value, 0f, maxBoost);
	}

	// Regen boostRegen per second until it's back to maxBoost
	[Range(0f, 1f)]
	[SerializeField] float boostRegen = 0.2f;
	public float BoostRegen { get => boostRegen;
		set => boostRegen = Mathf.Clamp01(value);
	}

	/*
		*  The force applied to the car when boosting
		*  NOTE: the boost does not care if the car is grounded or not
		*/
	[SerializeField] float boostForce = 10000;
	public float BoostForce { get => boostForce;
		set => boostForce = value;
	}

	// Use this for initialization
	void Start () {
        hasboost = true;
		///cap the boost
		if(boost > maxBoost)
		{
			boost = maxBoost;
		}
		//update ui elements
		UpdateUI();
	}
	

	public void UseBoost()
	{
		//reduce Boost level and update ui elements
		boost -= Time.deltaTime * boostConsumptionRate;
		
		// Analytics
		GameObject.Find("GameManager").GetComponent<AnalyticsController>().TotalBoost(Time.deltaTime * boostConsumptionRate);
	}

	public void UpdateUI()
	{
		boost += Time.deltaTime * boostRegen;
        if (boost > maxBoost) { boost = maxBoost; }
		boostIndicator.fillAmount = boost / 100;
		//if there is no Boost inform the user aaaa
		if(boost <=0)
		{
            hasboost = false;
			boost = 0;
            
		}
	}
}
