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

    private void Start()
    {
        defaultCrosshair.enabled = true;
        interactCrosshair.enabled = false;
        interactionText.text = "";
        interactionText.color = interactableTextColor;
    }

    public void SetNormal()
    {
        defaultCrosshair.enabled = true;
        interactCrosshair.enabled = false;
        interactionText.text = "";
    }

    public void SetInteractable(string prompt)
    {
        defaultCrosshair.enabled = false;
        interactCrosshair.enabled = true;
        interactionText.text = prompt;
        interactionText.color = interactableTextColor;
    }
}
