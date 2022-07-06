using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int money;
    [SerializeField] private AudioClip cashOut;
    //[SerializeField] private AudioClip 

    private AudioSource audioSource;

    public int Money
    {
        get => money;
        set => money = value;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
    }

    public void addMoney(int amount)
    {
        money += amount;
        GameObject.Find("GameManager").GetComponent<AnalyticsController>().TotalMoneyEarned(amount);
    }

    public void removeMoney(int amount)
    {
        audioSource.clip = cashOut;
        audioSource.Play();
        money -= amount;
        GameObject.Find("GameManager").GetComponent<AnalyticsController>().TotalMoneySpent(amount);
    }
}
