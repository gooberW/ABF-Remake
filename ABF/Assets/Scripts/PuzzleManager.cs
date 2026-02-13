using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public List<PuzzlePiece> pieces = new List<PuzzlePiece>(); // Assign 4 in Inspector
    private int snappedCount = 0;

    // Invoked when all pieces are snapped
    [Tooltip("Invoked when the puzzle is completed (all pieces snapped).")]
    public UnityEvent onPuzzleComplete;

    private void Awake()
    {
        if (onPuzzleComplete == null)
            onPuzzleComplete = new UnityEvent();
    }

    public void PieceSnapped()
    {
        snappedCount++;
        if (snappedCount == pieces.Count)
        {
            Debug.Log("Puzzle Complete!");
            // Win UI: Show full image, confetti, etc.
            onPuzzleComplete?.Invoke();
        }
    }

    public void ResetPuzzle()
    {
        foreach (var piece in pieces)
            piece.ResetPiece();
        snappedCount = 0;
    }
}