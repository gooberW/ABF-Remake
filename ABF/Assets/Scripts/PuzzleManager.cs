using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public PuzzlePiece[] pieces;
    public GameObject fullImageObject;
    public GameObject[] DialogueTriggerPuzzle = new GameObject[0];

    void Awake()
    {
        Instance = this;

        if (fullImageObject != null)
            fullImageObject.SetActive(false);

        
        if (DialogueTriggerPuzzle != null)
        {
            for (int i = 0; i < DialogueTriggerPuzzle.Length; i++)
            {
                if (DialogueTriggerPuzzle[i] != null)
                    DialogueTriggerPuzzle[i].SetActive(false);
            }
        }
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

        if (fullImageObject != null)
        {
            fullImageObject.SetActive(true);


            if (DialogueTriggerPuzzle != null)
            {
                for (int i = 0; i < DialogueTriggerPuzzle.Length; i++)
                    if (DialogueTriggerPuzzle[i] != null)
                        DialogueTriggerPuzzle[i].SetActive(true);
            }
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