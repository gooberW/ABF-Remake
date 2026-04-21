using System.Collections;
using UnityEngine;
using TMPro;

public class TaskTrigger : MonoBehaviour
{
    [Header("Task Settings")]
    public TaskSequence taskSequence;
    public float textSpeed = 0.05f;
    public TMP_Text toDoTask;
    public TMP_Text doneTask;
    public bool disableAfterTrigger = true;

    private bool isDisplaying = false;
    private Coroutine textCoroutine;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogWarning("No collider found on this GameObject. Adding BoxCollider.");
            triggerCollider = gameObject.AddComponent<BoxCollider>();
        }
        triggerCollider.isTrigger = true;
        doneTask.enabled = false;
        toDoTask.enabled = false;
    }

    private void Start()
    {
        if (taskSequence != null)
        {
            taskSequence.ResetSequence();
            DisplayCurrentTask();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDisplaying)
        {
            CheckTaskCompletion();
            if (disableAfterTrigger)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    public void CheckTaskCompletion()
    {
        var currentStep = taskSequence.GetCurrentStep();
        if (currentStep != null && !string.IsNullOrEmpty(currentStep.requiredTriggerTag) &&
            CompareTag(currentStep.requiredTriggerTag))
        {
            CompleteCurrentTask();
        }
        else
        {
            DisplayCurrentTask();
        }
    }

    public void DisplayCurrentTask()
    {
        var currentStep = taskSequence.GetCurrentStep();
        if (currentStep != null)
        {
            toDoTask.enabled = true;
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
            }
            textCoroutine = StartCoroutine(DisplayText(currentStep.description));
        }
    }

    public void CompleteCurrentTask()
    {
        // Mark previous task as done
        doneTask.text = toDoTask.text;
        doneTask.enabled = true;
        toDoTask.text = "";

        // Complete the step
        taskSequence.CompleteCurrentStep();

        // Display next task if available
        DisplayCurrentTask();
    }

    private IEnumerator DisplayText(string text)
    {
        isDisplaying = true;
        toDoTask.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            toDoTask.text = text.Substring(0, i + 1);
            yield return new WaitForSeconds(textSpeed);
        }

        isDisplaying = false;
    }
}