using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;          // The dialogue to trigger
    public bool triggerOnStart = false; // Should it play when the scene starts?
    public bool oneTimeTrigger = true;  // Should it only trigger once?
    private bool hasTriggered = false;  // Has it already triggered?

    private void Start()
    {
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !(oneTimeTrigger && hasTriggered))
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (oneTimeTrigger && hasTriggered) return;
        
        DialogueManager.Instance.StartDialogue(dialogue);
        hasTriggered = true;
    }
}