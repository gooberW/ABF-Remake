using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTaskOnMap : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private Collider taskcollider;
    private void Awake()
    {
        taskcollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;
        taskcollider.enabled = true;
    }
}
