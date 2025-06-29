using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerInteraction : MonoBehaviour, IInteractionHandler
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private CrosshairController crosshair;

    private Camera playerCamera;
    private IInteractable currentInteractable;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }

    private void CheckForInteractables()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null && interactable.IsInteractable)
            {
                if (interactable != currentInteractable)
                {
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnUnhover();
                    }

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
        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            currentInteractable.OnInteract();
            HandleInteraction(currentInteractable);
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