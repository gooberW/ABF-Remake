using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public List<PuzzlePiece> pieces = new List<PuzzlePiece>(); // Assign 4 in Inspector
    private int snappedCount = 0;

    public void PieceSnapped()
    {
        snappedCount++;
        if (snappedCount == pieces.Count)
        {
            Debug.Log("Puzzle Complete!");
            // Win UI: Show full image, confetti, etc.
        }
    }

    public void ResetPuzzle()
    {
        foreach (var piece in pieces)
            piece.ResetPiece();
        snappedCount = 0;
    }
}