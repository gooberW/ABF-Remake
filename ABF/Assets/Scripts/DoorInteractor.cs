using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private float interactDistance = 2.5f;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private string openPrompt = "Open Door";
    [SerializeField] private string closePrompt = "Close Door";

    private Door selectedDoor;

    void Update()
    {
        HandleDoorInteraction();
    }

    private void HandleDoorInteraction()
    {
        RaycastHit hit;
        bool hitDoor = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance, doorLayer);

        if (hitDoor)
        {
            Door door = hit.collider.GetComponent<Door>();

            if (door != null)
            {
                // Atualiza crosshair se mudou de porta
                if (door != selectedDoor)
                {
                    selectedDoor = door;
                    crosshairController.SetInteractable(door.IsDoorOpen ? closePrompt : openPrompt);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    door.ToggleDoor();
                    crosshairController.SetInteractable(door.IsDoorOpen ? closePrompt : openPrompt);
                }
            }
        }
        else if (selectedDoor != null)
        {
            crosshairController.SetNormal();
            selectedDoor = null;
        }
    }
}
