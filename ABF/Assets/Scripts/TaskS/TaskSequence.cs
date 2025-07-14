// TaskSequence.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Task Sequence", menuName = "Task System/Task Sequence")]
public class TaskSequence : ScriptableObject
{
    public List<TaskStep> steps;
    private int currentStepIndex = 0;

    public TaskStep GetCurrentStep()
    {
        if (currentStepIndex < steps.Count)
        {
            return steps[currentStepIndex];
        }
        return null;
    }

    public void CompleteCurrentStep()
    {
        if (currentStepIndex < steps.Count)
        {
            steps[currentStepIndex].isCompleted = true;
            currentStepIndex++;
        }
    }

    public void ResetSequence()
    {
        currentStepIndex = 0;
        foreach (var step in steps)
        {
            step.ResetStep();
        }
    }
}