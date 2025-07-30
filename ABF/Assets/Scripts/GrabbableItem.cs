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
    [SerializeField] private string warningText;

    public string InteractionPrompt => $"[{PlayerInteraction.interactButton}] Grab the {itemName}";
    public bool IsInteractable => true;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryScript>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
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
        }else
        {
            StartCoroutine(Warning());
        }
        
    }

    IEnumerator Warning()
    {
        Color color = warningTextComponent.color;
        warningTextComponent.color = new Color(color.r, color.g, color.b, 1f);
        warningTextComponent.text = "";
        foreach (char c in warningText)
        {
            warningTextComponent.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeOutText());
    }

    IEnumerator FadeOutText()
    {
        float elapsed = 0f;
        Color originalColor = warningTextComponent.color;

        while (elapsed < 1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            warningTextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        warningTextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure fully transparent
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
