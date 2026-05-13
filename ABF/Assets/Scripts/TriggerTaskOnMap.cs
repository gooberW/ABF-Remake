using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTaskOnMap : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private Collider taskcollider;
    [SerializeField] private GameObject spawnDialogue;
    [SerializeField] private GameObject taskObject;
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
        if (taskObject != null)
            taskObject.SetActive(true);
    }
}
