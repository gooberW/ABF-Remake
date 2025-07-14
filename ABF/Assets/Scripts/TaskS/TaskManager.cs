using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public TaskTrigger taskTrigger;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CheckItemTaskCompletion(GameObject item)
    {
        if (taskTrigger == null) return;
        
        var currentStep = taskTrigger.taskSequence.GetCurrentStep();
        if (currentStep != null && !string.IsNullOrEmpty(currentStep.requiredItemName) && 
            item.name.Contains(currentStep.requiredItemName))
        {
            taskTrigger.CompleteCurrentTask();
        }
    }
}