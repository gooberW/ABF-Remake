using UnityEngine;
using UnityEngine.Events;

public enum QTEType { Spam, Decision }

public class QTEManager : MonoBehaviour
{
    public SpamQTE spamQTE;
    public DecisionQTE decisionQTE;

    public bool isActive { get; private set; } = false;

    public void StartSpamQTE(UnityEvent customSuccess, UnityEvent customFailure)
    {
        if (isActive) return;
        isActive = true;
        spamQTE.onSuccess = customSuccess;
        spamQTE.onFailure = customFailure;
        spamQTE.StartQTE();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartDecisionQTE(KeyCode key1, string label1, UnityEvent onChoice1,
                                 KeyCode key2, string label2, UnityEvent onChoice2,
                                 UnityEvent onTimeout, string question)
    {
        if (isActive) return;
        isActive = true;
        decisionQTE.StartQTE(key1, label1, onChoice1, key2, label2, onChoice2, onTimeout, question);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EndQTE()
    {
        isActive = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}