using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DecisionQTE : MonoBehaviour
{
    public Canvas qteCanvas;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI questionText;
    public Image timerCircle;
    public float qteDuration = 3f;

    public UnityEvent onChoice1;
    public UnityEvent onChoice2;
    public UnityEvent onTimeout;

    private float timer;
    private bool isActive;
    private int chosenOption = -1;

    private KeyCode key1 = KeyCode.A;
    private KeyCode key2 = KeyCode.D;
    private string label1 = "Dodge Left";
    private string label2 = "Dodge Right";
    private string question = "Incoming Attack!";

    void Start()
    {
        ResetQTE();
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.unscaledDeltaTime;
        timerCircle.fillAmount = timer / qteDuration;

        if (Input.GetKeyDown(key1))
        {
            chosenOption = 0;
            ProcessChoice();
        }
        else if (Input.GetKeyDown(key2))
        {
            chosenOption = 1;
            ProcessChoice();
        }
        else if (timer <= 0)
        {
            Failure();
        }
    }

    public void StartQTE(KeyCode customKey1, string customLabel1, UnityEvent customOnChoice1,
                         KeyCode customKey2, string customLabel2, UnityEvent customOnChoice2,
                         UnityEvent customOnTimeout, string customQuestion)
    {
        key1 = customKey1;
        label1 = customLabel1;
        onChoice1 = customOnChoice1;

        key2 = customKey2;
        label2 = customLabel2;
        onChoice2 = customOnChoice2;

        onTimeout = customOnTimeout;
        question = customQuestion;

        ResetQTE();
        isActive = true;
        option1Text.text = $"{label1}";
        option2Text.text = $"{label2}";
        questionText.text = $"{question}";
        qteCanvas.gameObject.SetActive(true);
        option1Text.gameObject.SetActive(true);
        option2Text.gameObject.SetActive(true);
        timerCircle.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
    }

    private void ResetQTE()
    {
        isActive = false;
        timer = qteDuration;
        chosenOption = -1;
        timerCircle.fillAmount = 1f;
        qteCanvas.gameObject.SetActive(false);
        option1Text.gameObject.SetActive(false);
        option2Text.gameObject.SetActive(false);
        timerCircle.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
    }

    private void ProcessChoice()
    {
        if (chosenOption == 0)
        {
            onChoice1.Invoke();
        }
        else if (chosenOption == 1)
        {
            onChoice2.Invoke();
        }
        FindObjectOfType<QTEManager>().EndQTE();
        ResetQTE();
    }

    private void Failure()
    {
        onTimeout.Invoke();
        FindObjectOfType<QTEManager>().EndQTE();
        ResetQTE();
    }
}