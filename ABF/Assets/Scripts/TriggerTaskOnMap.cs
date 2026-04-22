using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTaskOnMap : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private Collider taskcollider;
    [SerializeField] private GameObject spawnDialogue;
    private void Awake()
    {
        taskcollider.enabled = false;
        if (spawnDialogue != null)
            spawnDialogue.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;
        taskcollider.enabled = true;
        if (spawnDialogue != null)
            spawnDialogue.SetActive(true);  
    }
}
