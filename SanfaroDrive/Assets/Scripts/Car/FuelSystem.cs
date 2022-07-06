using UnityEngine;

public class FuelSystem : MonoBehaviour {

    private bool hasFuel;
    private float fuel;
	private float maxFuel = 100f;  // max fuel
	[SerializeField] private float fuelConsumptionRate = 1;  // fuel drop rate

	public float FuelConsumptionRate
	{
		get => fuelConsumptionRate;
		set => fuelConsumptionRate = value;
	}

	public bool HasFuel
	{
		get => hasFuel;
		set => hasFuel = value;
	}

	public float Fuel
	{
		get => fuel;
		set => fuel = value;
	}


	// Use this for initialization
	void Start () {
        hasFuel = true;
        fuel = maxFuel;
		// update ui elements
		UpdateUI();
	}
	

	public void ReduceFuel()
	{
		// reduce fuel level and update ui elements
		fuel -= Time.deltaTime * fuelConsumptionRate;
		UpdateUI();

	}

	public void ReFuel(float fuel)
	{
		GameObject.Find("GameManager").GetComponent<AnalyticsController>().ReFuelTime();
		this.fuel = fuel;
		hasFuel = true;
		UpdateUI();
	}

	void UpdateUI()
	{
		// if there is no fuel inform the user
		if(fuel <=0)
		{
            hasFuel = false;
			fuel = 0;
		}
		GetComponent<FuelProgress>().Update();
	}
}
