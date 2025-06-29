using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineButtonController : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineToPlay;
    [SerializeField] private PlayableDirector timelineToStop;
    [SerializeField] private Button triggerButton;

    void Start()
    {
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(PlayTimeline);
        }
    }

    private void PlayTimeline()
    {
        if (timelineToStop != null && timelineToStop.state == PlayState.Playing)
        {
            timelineToStop.Stop();
        }

        if (timelineToPlay != null)
        {
            timelineToPlay.Stop(); // Restart from the beginning
            timelineToPlay.Play();
        }
    }
}
