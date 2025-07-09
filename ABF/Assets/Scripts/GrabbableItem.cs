using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//O codigo é reutilizavel para tudos os interactables, só meteres a layer "Interactables", adicionas este script ao object e já ta.

public class GrabbableItem : MonoBehaviour, IInteractable
{
    //Default Settings
    [SerializeField] private string itemName = "Item"; //Nome alteravel no inspector
    [SerializeField] private Transform grabPosition; //Assign Point no inspector
    //--------
    public string InteractionPrompt => $"[{PlayerInteraction.interactButton}] Grab the {itemName}";
    //------
    public bool IsInteractable => true;
    private Outline outline;
    private InventoryScript inventory;

    private void Start()
    {
        inventory = FindAnyObjectByType<InventoryScript>();    
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnHover()
    {
        if (!outline.enabled)
        {
            
            outline.enabled = true; 
        }
    }

    public void OnUnhover()
    {
        if (outline.enabled)
        {
            outline.enabled = false;
        }
    }

    public void OnInteract()
    {
        GrabItem();
    }
    
    private void GrabItem()
    {
        //Objeto vai pro Grab point do Inspector
        transform.SetParent(grabPosition);
        transform.localRotation = Quaternion.Euler(-90f, -90f, 0f); //Corrigir direção do objeto
        transform.localPosition = Vector3.zero;
        GetComponent<Collider>().isTrigger = true; //Tirar Colisão para não ficar preso nas paredes e etc
        gameObject.layer = LayerMask.NameToLayer("ItemInHand"); //Mudar layer para fazer a camera stack
        inventory.AddItem(gameObject);
    }
}
