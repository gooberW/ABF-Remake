using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoPuzzleInteractable : MonoBehaviour, IInteractable
{
    [Header("Configurações da Interação")]
    [SerializeField] private string promptMessage = "Examinar Foto";
    [SerializeField] private bool isInteractable = true;

    [Header("Configurações do Zoom (Objeto se move)")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float zoomDistance = 0.5f;
    [SerializeField] private float zoomDuration = 0.5f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Partes da Foto")]
    [SerializeField] private GameObject[] rippedParts;
    [SerializeField] private GameObject[] puzzleParts;
    [SerializeField] private GameObject photoFrame;

    [Header("Referências do Puzzle")]
    [SerializeField] private GameObject puzzleParent;

    [Header("Estado do Puzzle")]
    [SerializeField] private bool isPuzzleCompleted = false;

    // Interface IInteractable
    public string InteractionPrompt
    {
        get
        {
            if (isZoomed || isPuzzleCompleted) return "";
            return promptMessage;
        }
    }

    public bool IsInteractable => isInteractable && !isPuzzleCompleted && !isZoomed;

    // Variáveis privadas
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomed = false;
    private PlayerInteraction playerInteraction;
    private PlayerScript playerScript;
    private Outline outlineComponent;
    private Collider[] allFrameColliders;

    private void Start()
    {
        // Guardar posição original
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Configurar estado inicial das partes
        SetPuzzlePartsActive(false);
        SetRippedPartsActive(true);

        // Desativar puzzle parent se existir
        if (puzzleParent != null)
            puzzleParent.SetActive(false);

        // Referências
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (playerScript == null)
            playerScript = FindObjectOfType<PlayerScript>();

        if (playerInteraction == null)
            playerInteraction = FindObjectOfType<PlayerInteraction>();

        // Outline
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
            outlineComponent = GetComponentInChildren<Outline>();

        // Todos os colliders do frame
        allFrameColliders = GetComponentsInChildren<Collider>();

        // VERIFICAÇÕES DE DEBUG
        Debug.Log("=== VERIFICAÇÕES DO PHOTO PUZZLE ===");

        // Verificar Physics Raycaster na câmera
        PhysicsRaycaster raycaster = cameraTransform.GetComponent<PhysicsRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ CÂMERA SEM PHYSICS RAYCASTER! Adicionando automaticamente...");
            cameraTransform.gameObject.AddComponent<PhysicsRaycaster>();
        }
        else
        {
            Debug.Log("✅ Physics Raycaster encontrado na câmera");
        }

        // Verificar colliders das peças
        Debug.Log($"=== VERIFICANDO {puzzleParts.Length} PEÇAS ===");
        foreach (GameObject piece in puzzleParts)
        {
            if (piece != null)
            {
                Collider col = piece.GetComponent<Collider>();
                if (col == null)
                {
                    Debug.LogError($"❌ Peça {piece.name} NÃO TEM COLLIDER!");
                }
                else
                {
                    Debug.Log($"✅ Peça {piece.name} tem collider: {col.GetType()} - Layer: {LayerMask.LayerToName(piece.layer)}");
                }
            }
        }

        // Verificar layers
        Debug.Log($"Layer do photo frame: {LayerMask.LayerToName(gameObject.layer)}");
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
        Debug.Log("=== INICIANDO ZOOM NO PHOTO FRAME ===");

        isZoomed = true;
        isInteractable = false;

        // Desabilitar outline
        if (outlineComponent != null)
            outlineComponent.enabled = false;

        // Desabilitar TODOS os colliders do frame
        Debug.Log($"Desabilitando {allFrameColliders.Length} colliders do frame");
        foreach (Collider col in allFrameColliders)
        {
            if (col != null)
            {
                col.enabled = false;
                Debug.Log($"Collider {col.name} desabilitado");
            }
        }

        // Desabilitar movimento do jogador
        if (playerScript != null)
        {
            PlayerScript.CanMove = false;
            Debug.Log("Movimento do jogador desabilitado");
        }

        // Manter cursor invisível
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Guardar posição original
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Calcular posição alvo
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * zoomDistance;
        Quaternion targetRotation = originalRotation;

        // Animação de zoom
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsedTime / zoomDuration);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        // Atualizar estado das fotos
        SetRippedPartsActive(false);
        SetPuzzlePartsActive(true);

        if (puzzleParent != null)
            puzzleParent.SetActive(true);

        // FORÇAR REATIVAÇÃO DOS COLLIDERS DAS PEÇAS
        Debug.Log("Reativando colliders das peças...");
        foreach (GameObject piece in puzzleParts)
        {
            if (piece != null)
            {
                Collider col = piece.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                    col.enabled = true;
                    Debug.Log($"Collider de {piece.name} reativado");
                }
            }
        }

        // VERIFICAÇÃO FINAL
        Debug.Log("=== ZOOM COMPLETO ===");
        Debug.Log($"Peças ativas: {puzzleParts.Length}");
        Debug.Log($"Cursor invisível: {Cursor.visible}");
        Debug.Log($"Mouse position: {Input.mousePosition}");

        yield return new WaitForEndOfFrame();
    }

    public void ExitPhotoPuzzle()
    {
        if (isZoomed)
        {
            StartCoroutine(ReturnFromPhoto());
        }
    }

    private IEnumerator ReturnFromPhoto()
    {
        Debug.Log("=== SAINDO DO ZOOM ===");

        isZoomed = false;
        isInteractable = false;

        // Desativar puzzle
        SetPuzzlePartsActive(false);
        if (puzzleParent != null)
            puzzleParent.SetActive(false);

        // Animação de retorno
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

        // Reabilitar colliders do frame
        Debug.Log($"Reabilitando {allFrameColliders.Length} colliders do frame");
        foreach (Collider col in allFrameColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        // Reabilitar movimento
        if (playerScript != null)
        {
            PlayerScript.CanMove = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetRippedPartsActive(true);

        isInteractable = true;
        Debug.Log("=== ZOOM FINALIZADO ===");
    }

    private void SetRippedPartsActive(bool active)
    {
        foreach (GameObject part in rippedParts)
        {
            if (part != null)
                part.SetActive(active);
        }
    }

    private void SetPuzzlePartsActive(bool active)
    {
        foreach (GameObject part in puzzleParts)
        {
            if (part != null)
                part.SetActive(active);
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
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 targetPos = cameraTransform.position + cameraTransform.forward * zoomDistance;
            Gizmos.DrawWireSphere(targetPos, 0.1f);
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }

    private void OnDestroy()
    {
        if (isZoomed && playerScript != null)
        {
            PlayerScript.CanMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}