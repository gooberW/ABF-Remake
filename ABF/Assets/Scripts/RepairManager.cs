using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RepairManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject repairUI;
    public Image progressBar;
    public RectTransform needlePivot;
    public Image successZone;

    [Header("Skill Check Settings")]
    public float needleSpeed = 280f;
    public float successZoneWidth = 55f;      // Slightly wider - easier to test
    public int requiredSkillChecks = 8;

    [Header("Timing")]
    public float minTimeBetweenChecks = 1.4f;
    public float maxTimeBetweenChecks = 4.2f;

    [Header("Feedback")]
    public Image feedbackFlash;               // Add an Image (full screen or panel) for green/red flash
    public float flashDuration = 0.2f;

    private GeneratorInteractable currentGenerator;
    private int successfulChecks = 0;

    private float currentNeedleAngle = 0f;
    private float currentSuccessZoneCenter = 0f;
    private bool skillCheckActive = false;
    private bool wasInZoneLastFrame = false;   // For enter/exit detection

    public void StartRepair(GeneratorInteractable generator)
    {
        currentGenerator = generator;
        requiredSkillChecks = generator.requiredSkillChecks;

        successfulChecks = 0;
        progressBar.fillAmount = 0f;
        repairUI.SetActive(true);

        if (feedbackFlash != null)
            feedbackFlash.color = new Color(1, 1, 1, 0); // invisible at start

        StartCoroutine(RepairSequence());
    }

    private IEnumerator RepairSequence()
    {
        while (successfulChecks < requiredSkillChecks)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenChecks, maxTimeBetweenChecks));
            TriggerNewSkillCheck();
        }

        currentGenerator.OnRepairComplete();
        CloseRepairUI();
    }

    private void TriggerNewSkillCheck()
    {
        skillCheckActive = true;
        wasInZoneLastFrame = false;

        currentNeedleAngle = Random.Range(0f, 360f);
        currentSuccessZoneCenter = Random.Range(0f, 360f);

        if (successZone != null)
            successZone.rectTransform.localEulerAngles = new Vector3(0, 0, currentSuccessZoneCenter);

        Debug.Log($"<color=yellow>New Skill Check → Zone Center: {currentSuccessZoneCenter:F1}°</color>");
        StartCoroutine(SkillCheckTimer());
    }

    private void Update()
    {
        if (!skillCheckActive || needlePivot == null) return;

        currentNeedleAngle += needleSpeed * Time.deltaTime;
        currentNeedleAngle %= 360f;

        needlePivot.localEulerAngles = new Vector3(0, 0, -currentNeedleAngle);

        // === REAL-TIME ZONE CHECK (every frame) ===
        CheckIfNeedleInZone();
    }

    private void CheckIfNeedleInZone()
    {
        if (!skillCheckActive) return;

        float needleVisualAngle = -currentNeedleAngle;
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(needleVisualAngle, currentSuccessZoneCenter));
        bool isCurrentlyInZone = angleDiff <= (successZoneWidth / 2f);

        // Enter / Exit detection
        if (isCurrentlyInZone && !wasInZoneLastFrame)
        {
            Debug.Log($"<color=green>NEEDLE ENTERED ZONE</color> | Angle Diff: {angleDiff:F1}°");
        }
        else if (!isCurrentlyInZone && wasInZoneLastFrame)
        {
            Debug.Log($"<color=red>NEEDLE EXITED ZONE</color> | Angle Diff: {angleDiff:F1}°");
        }

        wasInZoneLastFrame = isCurrentlyInZone;
    }

    private IEnumerator SkillCheckTimer()
    {
        float timer = 0f;
        const float maxCheckDuration = 1.65f;

        while (skillCheckActive && timer < maxCheckDuration)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                skillCheckActive = false;

                float needleVisualAngle = -currentNeedleAngle;
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(needleVisualAngle, currentSuccessZoneCenter));
                bool isSuccess = angleDiff <= (successZoneWidth / 2f);

                if (isSuccess)
                {
                    successfulChecks++;
                    progressBar.fillAmount = (float)successfulChecks / requiredSkillChecks;
                    Debug.Log($"<color=green>✓ SUCCESS! Check {successfulChecks}/{requiredSkillChecks} | Diff: {angleDiff:F1}°</color>");
                    StartCoroutine(FlashFeedback(Color.green));
                }
                else
                {
                    currentGenerator.RegressProgress(0.12f);
                    Debug.Log($"<color=red>✗ FAILED - Missed zone | Diff: {angleDiff:F1}°</color>");
                    StartCoroutine(FlashFeedback(Color.red));
                }

                yield break;
            }

            yield return null;
        }

        // Time ran out
        if (skillCheckActive)
        {
            skillCheckActive = false;
            currentGenerator.RegressProgress(0.15f);
            Debug.Log("<color=red>✗ MISSED - Time expired</color>");
            StartCoroutine(FlashFeedback(Color.red));
        }
    }

    private IEnumerator FlashFeedback(Color color)
    {
        if (feedbackFlash == null) yield break;

        feedbackFlash.color = new Color(color.r, color.g, color.b, 0.4f); // semi-transparent flash
        yield return new WaitForSeconds(flashDuration);
        feedbackFlash.color = new Color(1, 1, 1, 0); // fade out
    }

    private void CloseRepairUI()
    {
        repairUI.SetActive(false);
        skillCheckActive = false;
    }

    public void CancelRepair()
    {
        StopAllCoroutines();
        CloseRepairUI();
        if (currentGenerator != null)
            currentGenerator.isRepairing = false;
    }
}