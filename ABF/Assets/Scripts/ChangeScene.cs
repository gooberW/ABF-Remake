using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public PlayableDirector timeline;
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(PlayThenChangeScene());
    }

    private IEnumerator PlayThenChangeScene()
    {
        timeline.Play();
        yield return new WaitUntil(() => timeline.state != PlayState.Playing);
        SceneManager.LoadScene(sceneName);
    }
}