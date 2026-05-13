using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gotomenu : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
