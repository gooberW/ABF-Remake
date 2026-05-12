using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayTimeline : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineplayer;
    [SerializeField] private bool blockPlayerMovement = true;
    [SerializeField] private bool isOneTimeTrigger = false;

    private bool _hasBeenTriggered = false;

    private void OnEnable()
    {
        if (timelineplayer != null)
        {
            timelineplayer.stopped += OnTimelineStopped;
        }
    }

    private void OnDisable()
    {
        if (timelineplayer != null)
        {
            timelineplayer.stopped -= OnTimelineStopped;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle one-time trigger functionality
        if (isOneTimeTrigger && _hasBeenTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            // Optionally block player movement while timeline plays
            if (blockPlayerMovement)
                PlayerScript.CanMove = false;

            timelineplayer.Play();

            // Mark as triggered for one-time functionality
            if (isOneTimeTrigger)
                _hasBeenTriggered = true;
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        // Optionally re-enable player movement when timeline finishes
        if (blockPlayerMovement)
            PlayerScript.CanMove = true;
    }
}
