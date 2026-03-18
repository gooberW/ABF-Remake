using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private Plane dragPlane;
    private Vector3 offset;
    private bool isDragging;
    public Vector3 correctPosition;
    public float snapDistance = 0.5f;
    private bool isPlaced;
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
            Debug.LogError("Camera.main não encontrada!");
    }

    void OnMouseDown()
    {
        if (isPlaced) return;

        // AUMENTAR A DISTÂNCIA DO RAYCAST DA CÂMARA
        Camera cam = Camera.main;
        cam.farClipPlane = 1000f; // Garantir que a câmera vê longe

        dragPlane = new Plane(cameraTransform.forward, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging || isPlaced) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + offset;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        Debug.Log($"🟢 OnMouseUp em {gameObject.name}");
        isDragging = false;

        if (Vector3.Distance(transform.position, correctPosition) < snapDistance)
        {
            transform.position = correctPosition;
            isPlaced = true;
            Debug.Log($"✨ Peça {gameObject.name} encaixada!");

            if (PuzzleManager.Instance != null)
                PuzzleManager.Instance.CheckCompletion();
        }
    }

    public bool IsPlaced() => isPlaced;
}