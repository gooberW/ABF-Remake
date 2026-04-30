using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    public bool disableAfterTrigger = true;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
            triggerCollider = gameObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TaskManager.Instance.CheckTriggerTaskCompletion(gameObject.tag);
            if (disableAfterTrigger) triggerCollider.enabled = false;
        }
    }
}