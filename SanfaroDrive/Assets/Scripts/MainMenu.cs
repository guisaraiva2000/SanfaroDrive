using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private CameraController cameraManagerUI;
    [SerializeField] private Button playBtn;
    
    void Start()
    {
        playBtn.Select();
        cameraManagerUI.setMainMenuCamera();
        Time.timeScale = 0f;
    }
    // Update is called once per frame
    public void Play()
    {
        mainMenuUI.SetActive(false);
        GameObject.Find("GameManager").GetComponent<MissionController>().EnableCarCanvas(true);
        Time.timeScale = 1f;
        cameraManagerUI.playCamera();
    }

    public void Settings(){
        SceneManager.LoadScene("SanfaroDrive");
    }

    public void QuitMenu(){
        Application.Quit();
    }

    public void FromPause(){
        playBtn.Select();
        cameraManagerUI.setMainMenuCamera();
    }
}