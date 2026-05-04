using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public float openAngle = 90f;
    public float openSpeed = 120f;
    public bool invertDirection = false;

    [Header("Lock")]
    public bool isLocked = false;
    [SerializeField] private string lockedPrompt = "Locked";

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private string openPrompt = "Open Door";
    [SerializeField] private string closePrompt = "Close Door";
    [SerializeField] private float interactDistance = 2.5f;

    [Header("Sons")]                                          
    [SerializeField] private AudioSource audioSource;        
    [SerializeField] private AudioClip openSound;           
    [SerializeField] private AudioClip closeSound;           

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
        bool hitDoor = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance, doorLayer);
        bool thisDoorHit = false;

        if (hitDoor && hit.collider != null)
        {
            Transform hitTransform = hit.collider.transform;
            if (hitTransform == this.transform || hitTransform.IsChildOf(this.transform))
                thisDoorHit = true;
        }

        if (thisDoorHit)
        {
            if (!isLookingAtDoor)
            {
                crosshairController.SetInteractable(GetCurrentPrompt());
                isLookingAtDoor = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!isLocked)
                {
                    ToggleDoor();
                    crosshairController.SetInteractable(GetCurrentPrompt());
                }
            }
        }
        else
        {
            if (isLookingAtDoor)
            {
                crosshairController.SetNormal();
                isLookingAtDoor = false;
            }
        }

        if (isMoving)
            MoveDoor();
    }

    private string GetCurrentPrompt()
    {
        if (isLocked) return lockedPrompt;
        return isDoorOpen ? closePrompt : openPrompt;
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
        if (isMoving || isLocked) return;
        isDoorOpen = !isDoorOpen;
        isMoving = true;

        AudioClip clip = isDoorOpen ? openSound : closeSound;
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void Unlock()
    {
        isLocked = false;
        if (isLookingAtDoor)
            crosshairController.SetInteractable(GetCurrentPrompt());
    }

    public void Lock()
    {
        isLocked = true;
        if (isLookingAtDoor)
            crosshairController.SetInteractable(GetCurrentPrompt());
    }
}