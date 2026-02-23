using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private Plane dragPlane;
    private Vector3 offset;
    private bool isDragging;
    public Vector3 correctPosition;
    public float snapDistance = 0.5f;
    private bool isPlaced;

    void OnMouseDown()
    {
        if (isPlaced) return;

        dragPlane = new Plane(Vector3.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        isDragging = false;

        if (Vector3.Distance(transform.position, correctPosition) < snapDistance)
        {
            transform.position = correctPosition;
            isPlaced = true;

            PuzzleManager.Instance.CheckCompletion();
        }
    }

    public bool IsPlaced() => isPlaced;
}
