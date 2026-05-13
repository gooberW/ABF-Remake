using UnityEngine;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    [Header("Cutscene Settings")]
    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private float cutsceneDuration = 5f;
    [SerializeField] private bool autoDisableAfterPlay = true;

    private static bool hasPlayed = false;
    private bool isPlaying = false;

    void Start()
    {
  
        if (playOnlyOnce && hasPlayed)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            PlayCutscene();
        }


            
    }

    void PlayCutscene()
    {
        if (isPlaying) return;

        isPlaying = true;
        hasPlayed = true;

        gameObject.SetActive(true);


        if (autoDisableAfterPlay)
        {
            StartCoroutine(DisableAfterCutscene());
        }
    }

    IEnumerator DisableAfterCutscene()
    {

        yield return new WaitForSeconds(cutsceneDuration);

        gameObject.SetActive(false);
    }

 
    public static void ResetCutsceneState()
    {
        hasPlayed = false;
    }

 
    public void ReactivateCutscene()
    {
        hasPlayed = false;
        gameObject.SetActive(true);
        PlayCutscene();
    }
}