using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionHandler
{
    void HandleHover(IInteractable interactable);
    void HandleUnhover();
    void HandleInteraction(IInteractable interactable);
}
