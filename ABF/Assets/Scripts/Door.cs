using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public float openAngle = 90f;           
    public float openSpeed = 120f;          
    public bool invertDirection = false;   

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private string openPrompt = "Open Door";
    [SerializeField] private string closePrompt = "Close Door";

    private Transform selectedDoor;
    private bool isLookingAtDoor = false;
    private bool isDoorOpen = false;
    public bool IsDoorOpen => isDoorOpen;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isMoving = false;

    void Start()
    {
        closedRotation = transform.localRotation;

        
        float direction = invertDirection ? -1f : 1f;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle * direction, 0f);
    }

    void Update()
    {
        RaycastHit hit;
        bool hitDoor = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.5f, doorLayer);

        if (hitDoor)
        {
            if (!isLookingAtDoor)
            {
                crosshairController.SetInteractable(isDoorOpen ? closePrompt : openPrompt);
                isLookingAtDoor = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ToggleDoor();
            }
        }
        else if (isLookingAtDoor)
        {
            crosshairController.SetNormal();
            isLookingAtDoor = false;
        }

        if (isMoving)
        {
            MoveDoor();
        }
    }

    private void MoveDoor()
    {
        Quaternion targetRot = isDoorOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRot, openSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.localRotation, targetRot) < 0.5f)
        {
            transform.localRotation = targetRot;
            isMoving = false;
        }
    }

    public void ToggleDoor()
    {
        if (isMoving) return; 
        isDoorOpen = !isDoorOpen;
        isMoving = true;

        crosshairController.SetInteractable(isDoorOpen ? closePrompt : openPrompt);
    }
}
