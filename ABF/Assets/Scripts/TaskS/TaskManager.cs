using UnityEngine;
using TMPro;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Task Settings")]
    public TaskSequence taskSequence;
    public float textSpeed = 0.05f;
    public TMP_Text toDoTask;
    public TMP_Text doneTask;

    private bool isDisplaying = false;
    private Coroutine textCoroutine;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        doneTask.enabled = false;
        toDoTask.enabled = false;
    }

    private void Start()
    {
        taskSequence.ResetSequence();
        DisplayCurrentTask();
    }

    public void CheckTriggerTaskCompletion(string triggerTag)
    {
        var currentStep = taskSequence.GetCurrentStep();
        if (currentStep == null) return;

        bool tagMatches = string.IsNullOrEmpty(currentStep.requiredTriggerTag) ||
                          currentStep.requiredTriggerTag == triggerTag;

        if (tagMatches) CompleteCurrentTask();
    }

    public void CheckItemTaskCompletion(GameObject item)
    {
        var currentStep = taskSequence.GetCurrentStep();
        if (currentStep != null && !string.IsNullOrEmpty(currentStep.requiredItemName) &&
            item.name.Contains(currentStep.requiredItemName))
        {
            CompleteCurrentTask();
        }
    }

    public void DisplayCurrentTask()
    {
        var currentStep = taskSequence.GetCurrentStep();
        if (currentStep == null) return;

        toDoTask.enabled = true;
        if (textCoroutine != null) StopCoroutine(textCoroutine);
        textCoroutine = StartCoroutine(DisplayText(currentStep.description));

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (currentStep.duration > 0f)
            timerCoroutine = StartCoroutine(TaskTimer(currentStep.duration));
    }

    public void CompleteCurrentTask()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            isDisplaying = false;
        }

        doneTask.text = toDoTask.text;
        doneTask.enabled = true;
        toDoTask.text = "";

        taskSequence.CompleteCurrentStep();
        DisplayCurrentTask();
    }

    private IEnumerator TaskTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        CompleteCurrentTask();
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