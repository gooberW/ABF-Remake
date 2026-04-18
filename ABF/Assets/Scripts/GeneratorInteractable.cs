using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GeneratorInteractable : MonoBehaviour, IInteractable
{
    [Header("Generator Settings")]
    public string interactionPrompt = "Repair Generator";
    public float repairTime = 60f;
    public int requiredSkillChecks = 8;              

    [Header("References")]
    public PlayableDirector enterTimeline;           
    public PlayableDirector exitTimeline;             
    public RepairManager repairManager;
    private Outline outline;

    private bool isRepairing = false;
    private float currentRepairProgress = 0f;

    public string InteractionPrompt => interactionPrompt;
    public bool IsInteractable => !isRepairing;

    public void OnHover()
    {
        
    }

    public void OnUnhover()
    {
       
    }

    public void OnInteract()
    {
        if (isRepairing) return;

        isRepairing = true;
        currentRepairProgress = 0f;

        if (enterTimeline != null)
            enterTimeline.Play();

        repairManager?.StartRepair(this);
    }

    public void OnRepairComplete()
    {
        isRepairing = false;
        if (exitTimeline != null)
            exitTimeline.Play();

        Debug.Log("Generator repaired!");
    }

    public void RegressProgress(float amount)
    {
        currentRepairProgress = Mathf.Max(0f, currentRepairProgress - amount);
    }
}