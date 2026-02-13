using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Config")]
    public Vector2 correctPosition;  // Set in Inspector per piece
    public float snapDistance = 50f; // Pixels to snap
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startPosition;
    private bool isSnapped = false;
    public PuzzleManager manager;    // Drag to Manager

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Optional: Bring to front (adjust sibling index)
        transform.SetAsLastSibling();
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, null, out localPos);
        rectTransform.anchoredPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isSnapped && Vector2.Distance(rectTransform.anchoredPosition, correctPosition) < snapDistance)
        {
            rectTransform.anchoredPosition = correctPosition;
            isSnapped = true;
            GetComponent<Image>().raycastTarget = false; // Lock
            manager?.PieceSnapped();
        }
        else
        {
            rectTransform.anchoredPosition = startPosition; // Reset
        }
    }

    public void ResetPiece()
    {
        isSnapped = false;
        rectTransform.anchoredPosition = startPosition;
        GetComponent<Image>().raycastTarget = true;
    }
}