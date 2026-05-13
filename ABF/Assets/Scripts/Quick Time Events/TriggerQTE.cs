using UnityEngine;
using UnityEngine.Events;

public class TriggerQTE : MonoBehaviour
{
    public QTEType qteType = QTEType.Spam;
    private QTEManager qteManager;

    public UnityEvent onQTE_Success;
    public UnityEvent onQTE_Failure;

    public KeyCode decisionKey1 = KeyCode.A;
    public string decisionLabel1 = "Dodge Left";
    public UnityEvent onDecisionChoice1;

    public KeyCode decisionKey2 = KeyCode.D;
    public string decisionLabel2 = "Dodge Right";
    public UnityEvent onDecisionChoice2;

    public UnityEvent onDecisionTimeout;
    public string decisionQuestion = "Incoming Attack!";

    void Awake()
    {
        qteManager = FindObjectOfType<QTEManager>();
    }

    public void StartQTE()
    {
        if (qteManager == null) qteManager = FindObjectOfType<QTEManager>();

        if (qteType == QTEType.Spam)
            qteManager.StartSpamQTE(onQTE_Success, onQTE_Failure);
        else
            qteManager.StartDecisionQTE(decisionKey1, decisionLabel1, onDecisionChoice1,
                                        decisionKey2, decisionLabel2, onDecisionChoice2,
                                        onDecisionTimeout, decisionQuestion);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (qteManager != null)
            {
                if (qteType == QTEType.Spam)
                    qteManager.StartSpamQTE(onQTE_Success, onQTE_Failure);
                else
                    qteManager.StartDecisionQTE(decisionKey1, decisionLabel1, onDecisionChoice1,
                                                decisionKey2, decisionLabel2, onDecisionChoice2,
                                                onDecisionTimeout, decisionQuestion);
            }
            else
            {
                Debug.LogError("TriggerQTE: QTEManager is missing!", this);
            }
        }
    }
}