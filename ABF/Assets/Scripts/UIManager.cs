using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public CutsceneController cutsceneController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        if (cutsceneController == null)
        {
            cutsceneController = FindObjectOfType<CutsceneController>();
        }
    }

    public void PlayGame()
    {

        CutsceneController.ResetCutsceneState();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}