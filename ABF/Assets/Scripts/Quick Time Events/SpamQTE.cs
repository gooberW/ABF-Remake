using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SpamQTE : MonoBehaviour
{
    public Canvas qteCanvas;
    public TextMeshProUGUI promptText;
    public Image progressCircle;
    public Image progressBackground;
    public KeyCode mashKey = KeyCode.E;
    public float qteDuration = 5f;
    public int requiredPresses = 20;
    public float decayRate = 0.5f;

    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    private float timer;
    private int pressCount;
    private bool isActive;

    void Start()
    {
        ResetQTE();
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.unscaledDeltaTime;

        pressCount -= (int)(decayRate * Time.unscaledDeltaTime);
        pressCount = Mathf.Max(0, pressCount);

        if (Input.GetKeyDown(mashKey))
        {
            pressCount++;
            progressCircle.color = Color.green;
        }

        progressCircle.fillAmount = (float)pressCount / requiredPresses;

        if (pressCount >= requiredPresses)
        {
            Success();
        }
        else if (timer <= 0)
        {
            Failure();
        }
    }

    public void StartQTE()
    {
        ResetQTE();
        isActive = true;
        promptText.text = $"{mashKey}";
        qteCanvas.gameObject.SetActive(true);
        promptText.gameObject.SetActive(true);
        progressCircle.gameObject.SetActive(true);
        progressBackground.gameObject.SetActive(true);
    }

    private void ResetQTE()
    {
        isActive = false;
        timer = qteDuration;
        pressCount = 0;
        progressCircle.fillAmount = 0f;
        progressCircle.color = Color.white;
        qteCanvas.gameObject.SetActive(false);

        promptText.gameObject.SetActive(false);
        progressCircle.gameObject.SetActive(false);
        progressBackground.gameObject.SetActive(false);
    }

    private void Success()
    {
        onSuccess.Invoke();
        FindObjectOfType<QTEManager>().EndQTE();
        ResetQTE();
    }

    private void Failure()
    {
        onFailure.Invoke();
        FindObjectOfType<QTEManager>().EndQTE();
        ResetQTE();
    }
}