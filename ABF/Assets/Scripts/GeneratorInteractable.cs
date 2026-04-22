using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class GeneratorInteractable : MonoBehaviour, IInteractable
{
    [Header("Generator Settings")]
    public CrosshairController crosshair;
    public string interactionPrompt = "Repair Generator";
    public float repairTime = 60f;          
    public int requiredSkillChecks = 8;

    [Header("References")]
    public RepairManager repairManager;
    public LightManager lightManager;
    public TMP_Text warningText;
    public string warningMessage = "The breaker is off, I need to fix the generator";


    public bool isRepairing = false;
    private Outline outline;
    public string InteractionPrompt => interactionPrompt;
    public bool IsInteractable => !isRepairing;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        if (lightManager.isGeneratorOff == true)
        {
            outline.enabled = true;
            if (warningText != null)
            {
                warningText.text = warningMessage;
                warningText.enabled = true;
            }
        }
        else
        {
            outline.enabled = false;
            if (warningText != null)
            {
                warningText.enabled = false;
            }
        }
    }

    public void OnHover()
    {
        crosshair.SetInteractable(interactionPrompt);
    }

    public void OnUnhover()
    {
        crosshair.SetNormal();
    }

    public void OnInteract()
    {
        if (isRepairing) return;

        isRepairing = true;

        repairManager.StartRepair(this);
    }

    public void OnRepairComplete()
    {
        isRepairing = false;
        lightManager.fused();
        lightManager.isGeneratorOff = false;

        Debug.Log("Generator repaired successfully!");
    }

    public void RegressProgress(float amount)
    {
        // You can expand this later if you want regression on the progress bar
        Debug.Log($"Progress regressed by {amount}");
    }
}