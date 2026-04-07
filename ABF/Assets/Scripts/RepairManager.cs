using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RepairManager : MonoBehaviour
{
    public GameObject repairUI;
    public Image progressBar;
    public RectTransform needlePivot;
    public Image successZone;

    public float needleSpeed = 290f;

    private GeneratorInteractable currentGenerator;
    private float currentAngle = 0f;
    private bool skillCheckActive = false;
    private int successfulChecks = 0;

    private bool needleIsInZone = false;

    public void StartRepair(GeneratorInteractable generator)
    {
        currentGenerator = generator;
        successfulChecks = 0;
        progressBar.fillAmount = 0f;
        repairUI.SetActive(true);
        needleIsInZone = false;

        StartCoroutine(RepairProgressLoop());
    }

    private IEnumerator RepairProgressLoop()
    {
        int required = currentGenerator.requiredSkillChecks;

        while (successfulChecks < required)
        {
            yield return new WaitForSeconds(Random.Range(1.6f, 4.8f));
            TriggerSkillCheck();
        }

        currentGenerator.OnRepairComplete();
        repairUI.SetActive(false);
    }

    private void TriggerSkillCheck()
    {
        skillCheckActive = true;
        needleIsInZone = false;

        currentAngle = Random.Range(0f, 360f);

        if (successZone)
            successZone.rectTransform.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

        Debug.Log("New Skill Check started");
        StartCoroutine(SkillCheckCoroutine());
    }

    private void Update()
    {
        if (skillCheckActive)
        {
            needlePivot.localEulerAngles = new Vector3(0, 0, -currentAngle);
            currentAngle += needleSpeed * Time.deltaTime;
            currentAngle %= 360f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cursor"))
        {
            needleIsInZone = true;
            Debug.Log("NEEDLE ENTERED ZONE");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cursor"))
        {
            needleIsInZone = false;
            Debug.Log("NEEDLE EXITED ZONE");
        }
    }

    private IEnumerator SkillCheckCoroutine()
    {
        float checkTimer = 0f;
        const float maxCheckTime = 1.7f;

        while (skillCheckActive && checkTimer < maxCheckTime)
        {
            checkTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                skillCheckActive = false;

                Debug.Log("Clicked - Needle in zone: " + needleIsInZone);

                if (needleIsInZone)
                {
                    successfulChecks++;
                    progressBar.fillAmount = (float)successfulChecks / currentGenerator.requiredSkillChecks;
                    Debug.Log("SUCCESS - Good skill check");
                }
                else
                {
                    currentGenerator.RegressProgress(0.12f);
                    progressBar.fillAmount = (float)successfulChecks / currentGenerator.requiredSkillChecks;
                    Debug.Log("FAILED - Needle not in zone");
                }

                yield break;
            }

            yield return null;
        }

        if (skillCheckActive)
        {
            skillCheckActive = false;
            currentGenerator.RegressProgress(0.15f);
            progressBar.fillAmount = (float)successfulChecks / currentGenerator.requiredSkillChecks;
            Debug.Log("MISSED (time ran out)");
        }
    }

    public void CancelRepair()
    {
        StopAllCoroutines();
        repairUI.SetActive(false);
        skillCheckActive = false;
    }
}