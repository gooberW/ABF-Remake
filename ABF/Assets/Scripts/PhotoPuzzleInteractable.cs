using System.Collections;
using UnityEngine;

public class PhotoPuzzleInteractable : MonoBehaviour, IInteractable
{
    [Header("Configurações da Interação")]
    [SerializeField] private string promptMessage = "Examinar Foto";
    [SerializeField] private bool isInteractable = true;

    [Header("Zoom")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float zoomDistance = 0.5f;
    [SerializeField] private float zoomDuration = 0.5f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Partes")]
    [SerializeField] private GameObject[] rippedParts;
    [SerializeField] private GameObject[] puzzleParts;
    [SerializeField] private GameObject photoFrame;

    [Header("Puzzle")]
    [SerializeField] private GameObject puzzleParent;

    [Header("Estado")]
    [SerializeField] private bool isPuzzleCompleted = false;

    public string InteractionPrompt
    {
        get
        {
            if (isZoomed || isPuzzleCompleted) return "";
            return promptMessage;
        }
    }

    public bool IsInteractable => isInteractable && !isPuzzleCompleted && !isZoomed;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomed = false;
    private PlayerScript playerScript;
    private Outline outlineComponent;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        SetPuzzlePartsActive(false);
        SetRippedPartsActive(true);

        if (puzzleParent != null)
            puzzleParent.SetActive(false);

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        playerScript = FindObjectOfType<PlayerScript>();

        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
            outlineComponent = GetComponentInChildren<Outline>();
    }

    public void OnHover()
    {
        if (IsInteractable && outlineComponent != null)
            outlineComponent.enabled = true;
    }

    public void OnUnhover()
    {
        if (outlineComponent != null)
            outlineComponent.enabled = false;
    }

    public void OnInteract()
    {
        if (!IsInteractable || isPuzzleCompleted) return;

        if (!isZoomed)
            StartCoroutine(ZoomIntoPhoto());
    }

    private IEnumerator ZoomIntoPhoto()
    {
        isZoomed = true;
        isInteractable = false;

        if (outlineComponent != null)
            outlineComponent.enabled = false;

        if (playerScript != null)
            PlayerScript.CanMove = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * zoomDistance;

        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsedTime / zoomDuration);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = originalRotation;

        SetRippedPartsActive(false);
        SetPuzzlePartsActive(true);

        if (puzzleParent != null)
            puzzleParent.SetActive(true);
    }

    public void ExitPhotoPuzzle()
    {
        if (isZoomed)
            StartCoroutine(ReturnFromPhoto());
    }

    private IEnumerator ReturnFromPhoto()
    {
        isZoomed = false;

        SetPuzzlePartsActive(false);
        if (puzzleParent != null)
            puzzleParent.SetActive(false);

        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsedTime / zoomDuration);

            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);

            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (playerScript != null)
            PlayerScript.CanMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetRippedPartsActive(true);

        isInteractable = true;
    }

    private void SetRippedPartsActive(bool active)
    {
        foreach (GameObject part in rippedParts)
        {
            if (part == null) continue;

            if (active)
                part.SetActive(true);
            else
                Destroy(part);
        }
    }

    private void SetPuzzlePartsActive(bool active)
    {
        foreach (GameObject part in puzzleParts)
        {
            if (part == null) continue;

            if (active)
                part.SetActive(true);
        }
    }

    public void CompletePuzzle()
    {
        isPuzzleCompleted = true;

        if (isZoomed)
        {
            StopAllCoroutines();
            StartCoroutine(ReturnFromPhoto());
        }

        TaskManager.Instance?.CompleteCurrentTask();
    }
}