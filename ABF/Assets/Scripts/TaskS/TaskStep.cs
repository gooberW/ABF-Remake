// TaskStep.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Task Step", menuName = "Task System/Task Step")]
public class TaskStep : ScriptableObject
{
    public string description;
    public string requiredItemName; // Leave empty if no item required
    public string requiredTriggerTag; // Leave empty if no trigger required
    public bool isCompleted;
    
    public void ResetStep()
    {
        isCompleted = false;
    }
}