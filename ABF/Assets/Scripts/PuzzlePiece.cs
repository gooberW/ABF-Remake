using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Referências")]
    public Collider frameCollider;

    [Header("Slot correto")]
    public Transform correctSlot;

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

    void Start()
    {
        myCollider = GetComponent<Collider>();

        // 🔥 calcular offset de rotação correto
        if (frameCollider != null)
        {
            initialRotationOffset =
                Quaternion.Inverse(frameCollider.transform.rotation) * transform.rotation;
        }
    }

    void Update()
    {
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == myCollider)
            {
                dragging = true;
                myCollider.enabled = false;
            }
        }
    }

    void Drag()
    {
        overFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == frameCollider)
            {
                overFrame = true;

                Vector3 targetPos =
                    hit.point + frameCollider.transform.forward * offset;

                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPos,
                    Time.deltaTime * smoothSpeed
                );

                // ✅ seguir inclinação da moldura
                transform.rotation =
                    frameCollider.transform.rotation * initialRotationOffset;

                // 🔥 magnetismo só quando está na moldura
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

        // 🔥 snap apenas se estiver na moldura
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

            // ✅ rotação perfeita (sem bug 90°)
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