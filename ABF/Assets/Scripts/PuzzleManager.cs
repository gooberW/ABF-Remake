using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public PuzzlePiece[] pieces;
    public GameObject fullImageObject;
    public GameObject TaskTriggerKitchen;
    public GameObject DialogueTriggerPuzzle;

    void Awake()
    {
        Instance = this;

        if (fullImageObject != null)
            fullImageObject.SetActive(false); // começa escondida

            if (TaskTriggerKitchen != null)
            TaskTriggerKitchen.SetActive(false);

            if (DialogueTriggerPuzzle != null)
            DialogueTriggerPuzzle.SetActive(false);
    }

    public void CheckCompletion()
    {
        foreach (var piece in pieces)
        {
            if (!piece.IsPlaced())
                return;
        }

        PuzzleCompleted();
    }

    void PuzzleCompleted()
    {
        Debug.Log("Puzzle Complete!");

        // ✅ mostrar imagem final
        if (fullImageObject != null)
        {
            fullImageObject.SetActive(true);

            // opcional: alinhar com puzzle
            fullImageObject.transform.position = pieces[0].transform.position;
            fullImageObject.transform.rotation = pieces[0].transform.rotation;
            TaskTriggerKitchen.SetActive(true);
            DialogueTriggerPuzzle.SetActive(true);
        }

        foreach (var piece in pieces)
        {
            piece.gameObject.SetActive(false);
        }

        PhotoPuzzleInteractable photoPuzzle = FindObjectOfType<PhotoPuzzleInteractable>();
        if (photoPuzzle != null)
        {
            photoPuzzle.CompletePuzzle();
        }
    }
}