using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VehicleBehaviour;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private Button resumeBtn;
    
    private bool gameIsPaused;
    private MissionController missionController;

    private void Start()
    {
        missionController = GameObject.Find("GameManager").GetComponent<MissionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button9)){
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void Resume(){
        pauseMenuUI.SetActive(false);
        missionController.EnableCarCanvas(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
    
    void Pause(){
        missionController.EnableCarCanvas(false);
        missionController.SelectedCar.GetComponent<EngineSoundManager>().MuteSounds(true);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;  
        resumeBtn.Select();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("SanfaroDrive");
    }

    public void QuitMenu(){
        pauseMenuUI.SetActive(false);
        missionController.EnableCarCanvas(false);
        mainMenuUI.gameObject.SetActive(true);
        gameIsPaused = false;
        mainMenuUI.GetComponentInChildren<MainMenu>().FromPause();
    }
}