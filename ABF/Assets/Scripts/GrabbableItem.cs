using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//O codigo é reutilizavel para tudos os interactables, só meteres a layer "Interactables", adicionas este script ao object e já ta.

public class GrabbableItem : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Transform grabPosition;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private float dropForce = 5f;

    private Outline outline;
    private InventoryScript inventory;
    private Rigidbody rb;
    private Camera cam;
    private bool isGrabbed = false;
    [SerializeField] private TMP_Text warningTextComponent;
    private string warningText = "I'm already carrying too much!";

    private bool isWarningActive = false; // Add this flag to track warning state
    private Coroutine warningCoroutine; // Track the current warning coroutine 

    public string InteractionPrompt => $"[{PlayerInteraction.interactButton}] Grab the {itemName}";
    public bool IsInteractable => true;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryScript>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        GameObject grabObj = GameObject.FindGameObjectWithTag("GrabPoint");
        if (grabObj != null)
            grabPosition = grabObj.transform;
        else
            Debug.LogWarning("No object with tag 'GrabPoint' found!");

        GameObject warningObj = GameObject.FindGameObjectWithTag("Warning");
        if (warningObj != null)
            warningTextComponent = warningObj.GetComponent<TMP_Text>();
        else
            Debug.LogWarning("No object with tag 'Warning' found!");
    }

    public void OnHover()
    {
        if (!outline.enabled) outline.enabled = true;
    }

    public void OnUnhover()
    {
        if (outline.enabled) outline.enabled = false;
    }

    public void OnInteract()
    {
        if (isGrabbed) ReleaseItem();
        else GrabItem();
    }

    private void GrabItem()
    {
        if (!inventory.IsInventoryFull())
        {
            isGrabbed = true;
            transform.SetParent(grabPosition);
            transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);
            transform.localPosition = Vector3.zero;

            GetComponent<Collider>().isTrigger = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            gameObject.layer = LayerMask.NameToLayer("ItemInHand");
            inventory.AddItem(gameObject, itemSprite, itemName);

            TaskManager.Instance.CheckItemTaskCompletion(gameObject);
        }
        else
        {
            // Only show warning if one isn't already active
            if (!isWarningActive)
            {
                StartCoroutine(Warning());
            }
        }
    }

    IEnumerator Warning()
    {
        isWarningActive = true;

        // Stop any existing warning coroutine
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }
        warningCoroutine = StartCoroutine(WarningRoutine());

        yield return warningCoroutine;

        isWarningActive = false;
        warningCoroutine = null;
    }

    IEnumerator WarningRoutine()
    {
        Color color = warningTextComponent.color;
        warningTextComponent.color = new Color(color.r, color.g, color.b, 1f);
        warningTextComponent.text = "";

        // Type out the text
        foreach (char c in warningText)
        {
            warningTextComponent.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2f);

        // Fade out the text
        float elapsed = 0f;
        Color originalColor = warningTextComponent.color;

        while (elapsed < 1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            warningTextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        warningTextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    public void ReleaseItem()
    {
        isGrabbed = false;
        GetComponent<Collider>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Interactable");

        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;

        Vector3 throwDirection = cam.transform.forward;
        rb.AddForce(throwDirection * dropForce, ForceMode.Impulse);

        inventory.RemoveItem(gameObject);
    }
}
