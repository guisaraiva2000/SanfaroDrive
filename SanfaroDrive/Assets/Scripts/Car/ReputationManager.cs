using UnityEngine;
using UnityEngine.UI;


public class ReputationManager : MonoBehaviour
{

    [SerializeField] private AudioClip clip;
    [SerializeField] private Image stars;
    private int reputation;
    private AudioSource audioSource;
    
    private const int  reputationAmountRate = 11;

    public int Reputation
    {
        get => reputation;
        set => reputation = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clip;
        
        reputation = 33;
    }

    // Update is called once per frame
    void Update()
    {
        stars.fillAmount = (float) reputation / 100;
    }

    public void addReputation(){
        if (reputation + reputationAmountRate <= 100)
        {
            reputation += reputationAmountRate;
        }

        if (reputation is 33 or 66 or 100 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void removeReputation(){
        if (reputation - reputationAmountRate >= 0)
        {
            reputation -= reputationAmountRate;
        }
    }
}