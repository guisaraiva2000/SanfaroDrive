using System;
using System.IO;
using UnityEngine;

public class AnalyticsController : MonoBehaviour
{
    private string[] output;
    private StreamWriter writer;
    private float startTime;
    private float timestamp;

    // Metrics
    private int completedTaxiMissions;
    private int completedMafiaMission;
    private int totalTaxiMissions;
    private int totalMafiaMission;
    private float completionTimeTaxiMission;
    private float expectedTimeTaxiMission;
    private float completionTimeMafiaMission;
    private float expectedTimeMafiaMission;
    private float totalTimeSpentTaxiMission;
    private float totalTimeSpentMafiaMission;
    private float totalFuelPerMission;
    private float reFuelTime;
    private int noFuelCounter;
    private int moneySpentOnFuel;
    private int totalDrifts;
    private int totalJumps;
    private float totalBoost;
    private int totalVehicleChanges;
    private int workshopVisits;
    private int moneySpentOnWorkshop;
    private int totalMoneyEarned;
    private int totalMoneySpent;
    private Vector3[] heatMap;

    // Start is called before the first frame update
    void Start()
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        
        // path = Application.dataPath + "/" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".json";
        //writer = new StreamWriter(path, true);
        //writer.WriteLine("{\n");
        startTime = Time.unscaledTime;
    }

    private void OnApplicationQuit()
    {
        //writer.WriteLine("}");
        //writer.Close();
    }
    
    void WriteJson(String name, String value)
    {
        timestamp = Time.unscaledTime - startTime;
        //writer.WriteLine("\t{" + "'Timestamp': " + timestamp.ToString("0.000") + ", '" + name + "': " + value + "},");
    }
    
    // Metrics functions
    
    public void CompletedTaxiMissions()
    {
       completedTaxiMissions++;
       WriteJson("CompletedTaxiMissions", completedTaxiMissions.ToString());
    }
   
   public void CompletedMafiaMissions()
   {
       completedMafiaMission++;
       WriteJson("CompletedMafiaMission", completedMafiaMission.ToString());
   }

   public void TotalTaxiMissions()
   {
       totalTaxiMissions++;
       WriteJson("TotalTaxiMissions", totalTaxiMissions.ToString());
   }

   public void TotalMafiaMission()
   {
       totalMafiaMission++;
       WriteJson("TotalMafiaMission", totalMafiaMission.ToString());
   }

   public void CompletionTimeTaxiMission(float time)
   {
       completionTimeTaxiMission += time;
       WriteJson("CompletionTimeTaxiMission", completionTimeTaxiMission.ToString("0.000"));
   }

   public void ExpectedTimeTaxiMission(float time)
   {
       expectedTimeTaxiMission += time;
       WriteJson("ExpectedTimeTaxiMission", expectedTimeTaxiMission.ToString("0.000"));
   }

   public void CompletionTimeMafiaMission(float time)
   {
       completionTimeMafiaMission += time;
       WriteJson("CompletionTimeMafiaMission", completionTimeMafiaMission.ToString("0.000"));
   }

   public void ExpectedTimeMafiaMission(float time)
   {
       expectedTimeMafiaMission += time;
       WriteJson("ExpectedTimeMafiaMission", expectedTimeMafiaMission.ToString("0.000"));
   }
   
   public void TotalTimeSpentTaxiMission(float time)
   {
       totalTimeSpentTaxiMission += time;
       WriteJson("TotalTimeSpentTaxiMission", totalTimeSpentTaxiMission.ToString("0.000"));
   }
   
   public void TotalTimeSpentMafiaMission(float time)
   {
       totalTimeSpentMafiaMission += time;
       WriteJson("TotalTimeSpentMafiaMission", totalTimeSpentMafiaMission.ToString("0.000"));
   }

   public void TotalFuelPerMission(float fuel)
   {
       totalFuelPerMission += fuel;
       WriteJson("TotalFuelPerMission", totalFuelPerMission.ToString("0.000"));
   }

   public void ReFuelTime()
   {
       reFuelTime = timestamp - reFuelTime;
       WriteJson("ReFuelTime", reFuelTime.ToString("0.000"));
   }

   public void NoFuelCounter()
   {
       noFuelCounter++;
       WriteJson("NoFuelCounter", noFuelCounter.ToString());
   }
   
   public void MoneySpentOnFuel(int money)
   {
       moneySpentOnFuel += money;
       WriteJson("MoneySpentOnFuel", moneySpentOnFuel.ToString());
   }

   public void TotalDrifts()
   {
       totalDrifts++;
       WriteJson("TotalDrifts", totalDrifts.ToString());
   }

   public void TotalJumps()
   {
       totalJumps++;
       WriteJson("TotalJumps", totalJumps.ToString());
   }

   public void TotalBoost(float boost)
   {
       totalBoost += boost;
       WriteJson("TotalBoost", totalBoost.ToString("0.000"));
   }
   
   public void TotalVehicleChanges()
   {
       totalVehicleChanges++;
       WriteJson("TotalVehicleChanges", totalVehicleChanges.ToString());
   }
   
   public void WorkshopVisits()
   {
       workshopVisits++;
       WriteJson("WorkshopVisits", workshopVisits.ToString());
   }

   public void MoneySpentOnWorkshop(int money)
   {
       moneySpentOnWorkshop += money;
       WriteJson("MoneySpentOnWorkshop", moneySpentOnWorkshop.ToString());
   }
   
   public void TotalMoneyEarned(int money)
   {
       totalMoneyEarned += money;
       WriteJson("TotalMoneyEarned", totalMoneyEarned.ToString());
   }
   
   public void TotalMoneySpent(int money)
   {
       totalMoneySpent += money;
       WriteJson("TotalMoneySpent", totalMoneySpent.ToString());
   }
   
   public void HeatMap(Vector3 position)
   {
       WriteJson("HeatMap", position.ToString());
   }
}
