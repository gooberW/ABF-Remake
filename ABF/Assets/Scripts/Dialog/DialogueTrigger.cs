using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool triggerOnStart = false;
    public bool oneTimeTrigger = true;
    public UnityEvent onDialogueComplete;
    private bool hasTriggered = false;

    private void Start()
    {
        if (triggerOnStart) TriggerDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !(oneTimeTrigger && hasTriggered))
            TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (oneTimeTrigger && hasTriggered) return;
        DialogueManager.Instance.StartDialogue(dialogue, onDialogueComplete); // <-- pass it
        hasTriggered = true;
    }
}