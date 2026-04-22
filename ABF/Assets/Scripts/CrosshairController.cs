using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Image defaultCrosshair;  
    [SerializeField] private Image interactCrosshair;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private Color interactableTextColor = Color.white;

    private void Awake()
    {
        if (defaultCrosshair == null || interactCrosshair == null || interactionText == null)
            Debug.LogWarning("CrosshairController: missing inspector references.");

        SetNormal();
    }

    public void SetNormal()
    {
        if (defaultCrosshair != null) defaultCrosshair.gameObject.SetActive(true);
        if (interactCrosshair != null) interactCrosshair.gameObject.SetActive(false);
        if (interactionText != null) interactionText.text = "";
    }

    public void SetInteractable(string prompt)
    {
        if (defaultCrosshair != null) defaultCrosshair.gameObject.SetActive(false);
        if (interactCrosshair != null) interactCrosshair.gameObject.SetActive(true);
        if (interactionText != null)
        {
            interactionText.text = prompt;
            interactionText.color = interactableTextColor;
        }
    }
}
