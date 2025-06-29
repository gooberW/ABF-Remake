using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string InteractionPrompt { get; }  
    bool IsInteractable { get; }      

    void OnHover();     
    void OnUnhover();   
    void OnInteract();  
}
