using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Referências")]
    public Collider frameCollider;

    [Header("Slot correto")]
    public Transform correctSlot;

    [Header("Câmera (opcional)")]
    [Tooltip("Assign the camera you want to use for dragging. If left empty, it will use Camera.main")]
    public Camera puzzleCamera;

    [Header("Arrasto")]
    public float offset = 0.01f;
    public float smoothSpeed = 18f;

    [Header("Magnetismo")]
    public float magnetDistance = 0.3f;
    public float magnetSpeed = 12f;
    public float snapDistance = 0.1f;

    private bool dragging;
    private bool placed;
    private bool overFrame;

    private Collider myCollider;
    private Quaternion initialRotationOffset;

    private void Start()
    {
        myCollider = GetComponent<Collider>();

        if (frameCollider != null)
        {
            initialRotationOffset = Quaternion.Inverse(frameCollider.transform.rotation) * transform.rotation;
        }

        // If no camera is assigned, use Camera.main
        if (puzzleCamera == null)
        {
            puzzleCamera = Camera.main;
            Debug.Log($"[PuzzlePiece] {gameObject.name}: No camera assigned, using Camera.main", this);
        }
    }

    private void Update()
    {
        // Debug: show which camera we're using
        if (puzzleCamera != null)
            Debug.Log($"Using camera: {puzzleCamera.gameObject.name}");

        if (placed) return;

        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (Input.GetMouseButton(0) && dragging)
            Drag();

        if (Input.GetMouseButtonUp(0) && dragging)
            StopDrag();
    }

    void TryStartDrag()
    {
        if (puzzleCamera == null) return;

        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"[TryStartDrag] Hit: {hit.collider.gameObject.name}");

            if (hit.collider == myCollider)
            {
                dragging = true;
                myCollider.enabled = false;
                Debug.Log($"<color=green>DRAG STARTED on {gameObject.name}</color>");
            }
        }
        else
        {
            Debug.Log("[TryStartDrag] Raycast missed everything");
        }
    }

    void Drag()
    {
        if (puzzleCamera == null) return;

        overFrame = false;
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == frameCollider)
            {
                overFrame = true;

                Vector3 targetPos = hit.point + frameCollider.transform.forward * offset;

                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPos,
                    Time.deltaTime * smoothSpeed
                );

                transform.rotation = frameCollider.transform.rotation * initialRotationOffset;

                // Magnetism
                if (correctSlot != null)
                {
                    float dist = Vector3.Distance(hit.point, correctSlot.position);

                    if (dist < magnetDistance)
                    {
                        transform.position = Vector3.Lerp(
                            transform.position,
                            correctSlot.position,
                            Time.deltaTime * magnetSpeed
                        );

                        transform.rotation = Quaternion.Lerp(
                            transform.rotation,
                            correctSlot.rotation * initialRotationOffset,
                            Time.deltaTime * magnetSpeed
                        );

                        if (dist < snapDistance)
                        {
                            SnapPiece();
                        }
                    }
                }
            }
        }
    }

    void StopDrag()
    {
        dragging = false;
        myCollider.enabled = true;

        if (correctSlot != null && overFrame)
        {
            float dist = Vector3.Distance(transform.position, correctSlot.position);
            if (dist < snapDistance)
            {
                SnapPiece();
            }
        }
    }

    void SnapPiece()
    {
        placed = true;
        dragging = false;

        if (correctSlot != null)
        {
            transform.position = correctSlot.position;
            transform.rotation = correctSlot.rotation * initialRotationOffset;
        }

        if (myCollider != null)
            myCollider.enabled = true;

        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.CheckCompletion();
    }

    public bool IsPlaced()
    {
        return placed;
    }
}