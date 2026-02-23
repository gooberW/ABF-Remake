using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public PuzzlePiece[] pieces;
    public GameObject fullImageObject;

    void Awake()
    {
        Instance = this;
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

        fullImageObject.SetActive(true);

        foreach (var piece in pieces)
        {
            piece.gameObject.SetActive(false);
        }
    }
}
