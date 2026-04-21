using UnityEngine;
using UnityEngine.Playables;

public class GeneratorInteractable : MonoBehaviour, IInteractable
{
    [Header("Generator Settings")]
    public string interactionPrompt = "Repair Generator";
    public float repairTime = 60f;           // Not directly used now, but kept for future
    public int requiredSkillChecks = 8;

    [Header("References")]
    public PlayableDirector enterTimeline;
    public PlayableDirector exitTimeline;
    public RepairManager repairManager;
    public LightManager lightManager;

    public bool isRepairing = false;

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

        if (enterTimeline != null)
            enterTimeline.Play();

        repairManager.StartRepair(this);
    }

    public void OnRepairComplete()
    {
        isRepairing = false;
        if (exitTimeline != null)
            exitTimeline.Play();
        lightManager.fused();

        Debug.Log("Generator repaired successfully!");
    }

    public void RegressProgress(float amount)
    {
        // You can expand this later if you want regression on the progress bar
        Debug.Log($"Progress regressed by {amount}");
    }
}