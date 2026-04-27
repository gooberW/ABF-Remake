using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;
    public GameObject SettingsUI;
    public GameObject AudioUI;
    public GameObject GameplayUI;

    [Header("Player Scripts")]
    public CameraScript cameraScript;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
        AudioUI.SetActive(false);
        GameplayUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        SettingsUI.SetActive(false);
        AudioUI.SetActive(false);
        GameplayUI.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraScript != null)
            cameraScript.isPaused = true;

        isPaused = true;
    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
        AudioUI.SetActive(false);
        GameplayUI.SetActive(false);
    }

    public void Audio()
    {
        pauseMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
        AudioUI.SetActive(true);
        GameplayUI.SetActive(false);
    }

    public void Gameplay()
    {
        pauseMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
        AudioUI.SetActive(false);
        GameplayUI.SetActive(true);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
        AudioUI.SetActive(false);
        GameplayUI.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraScript != null)
            cameraScript.isPaused = false; 

        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}