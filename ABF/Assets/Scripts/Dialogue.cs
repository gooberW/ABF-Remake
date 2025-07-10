using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;  // Array of dialogue lines
    public Dialogue nextDialogue; // (Optional) Link to the next dialogue if needed
}