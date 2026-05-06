using UnityEngine;

[CreateAssetMenu(fileName = "New Task Step", menuName = "Task System/Task Step")]
public class TaskStep : ScriptableObject
{
    public string description;
    public string requiredItemName;
    public string requiredTriggerTag;
    public float duration = 0f;
    public bool isCompleted;

    public void ResetStep()
    {
        isCompleted = false;
    }
}