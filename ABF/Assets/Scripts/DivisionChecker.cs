using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DivisionChecker : MonoBehaviour
{
    [Header("Division Settings")]
    public float textSpeed = 0.05f;
    public float disappearSpeed = 0.03f;
    public TMP_Text textComponent;
    public float displayDurationAfterExit = 3f;
    public bool disableAfterTrigger = true;

    private bool isDisplaying = false;
    private bool isDisappearing = false;
    private bool hasBeenTriggered = false;
    private Coroutine textCoroutine;
    private Collider triggerCollider;
    private string fullText = "";

    //[SerializeField] private TMP_Text divisionName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Division") && gameObject.CompareTag("Player"))
        {
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
            }
            textCoroutine = StartCoroutine(DisplayText(other.gameObject.name));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Division") && gameObject.CompareTag("Player"))
        {
            Debug.Log("Exited Trigger");
            StartCoroutine(StartDisappearingAfterDelay());
        }
        
    }

    private IEnumerator DisplayText(string DivisionName)
    {
        isDisplaying = true;
        textComponent.text = "";
        fullText = DivisionName;

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(textSpeed);
        }

        isDisplaying = false;
        hasBeenTriggered = true;

        if (disableAfterTrigger)
        {
            StartCoroutine(StartDisappearingAfterDelay());
        }
    }

    private IEnumerator StartDisappearingAfterDelay()
    {
        yield return new WaitForSeconds(displayDurationAfterExit);

        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = StartCoroutine(DisappearText());
    }

    private IEnumerator DisappearText()
    {
        isDisappearing = true;
        int currentLength = textComponent.text.Length;

        while (currentLength > 0)
        {
            currentLength--;
            textComponent.text = fullText.Substring(0, currentLength);
            yield return new WaitForSeconds(disappearSpeed);
        }

        textComponent.text = "";
        isDisappearing = false;

        if (disableAfterTrigger)
        {
            triggerCollider.enabled = false;
        }
    }

}
