using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerInteraction : MonoBehaviour, IInteractionHandler
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("References")]
    [SerializeField] private CrosshairController crosshair;

    public static KeyCode interactButton = KeyCode.E;
    public static KeyCode releaseButton = KeyCode.Q;

    private Camera playerCamera;
    private IInteractable currentInteractable;
    private InventoryScript inventory;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        inventory = FindObjectOfType<InventoryScript>();
    }

    private void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }

    private void CheckForInteractables()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            // Check for task triggers first
            TaskTrigger taskTrigger = hit.collider.GetComponent<TaskTrigger>();
            if (taskTrigger != null)
            {
                taskTrigger.CheckTaskCompletionByTrigger();
                return;
            }

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.IsInteractable)
            {
                if (interactable != currentInteractable)
                {
                    currentInteractable?.OnUnhover();
                    currentInteractable = interactable;
                    interactable.OnHover();
                    HandleHover(interactable);
                }
                return;
            }
        }

        if (currentInteractable != null)
        {
            currentInteractable.OnUnhover();
            HandleUnhover();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactButton)) currentInteractable?.OnInteract();
        if (Input.GetKeyDown(releaseButton)) ReleaseHeldItem();
    }

    private void ReleaseHeldItem()
    {
        GameObject heldItem = inventory.GetCurrentItem();
        if (heldItem == null) return;

        if (heldItem.TryGetComponent(out GrabbableItem grabbableItem))
        {
            grabbableItem.ReleaseItem();
        }
    }

    public void HandleHover(IInteractable interactable)
    {
        crosshair.SetInteractable(interactable.InteractionPrompt);
    }

    public void HandleUnhover()
    {
        crosshair.SetNormal();
    }

    public void HandleInteraction(IInteractable interactable)
    {
        Debug.Log($"Interacted with: {interactable.InteractionPrompt}");
    }
}