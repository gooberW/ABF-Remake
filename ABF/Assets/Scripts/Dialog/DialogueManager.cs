using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }  // Singleton pattern

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;  // The dialogue box UI
    [SerializeField] private TMP_Text characterNameText;    // Displays the character's name
    [SerializeField] private TMP_Text dialogueText;        // Displays the dialogue text

    private Dialogue currentDialogue;  // The currently active dialogue
    private int currentLineIndex;      // Tracks which line we're on
    private bool isDialogueActive;     // Is dialogue currently playing?

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes (optional)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Starts a new dialogue
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }

    // Displays the current line of dialogue
    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Length) return;

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        characterNameText.text = line.characterName;
        dialogueText.text = line.text;
    }

    // Advances to the next line (or ends dialogue)
    public void ContinueDialogue()
    {
        if (!isDialogueActive) return;

        currentLineIndex++;
        
        if (currentLineIndex < currentDialogue.lines.Length)
        {
            DisplayCurrentLine();  // Show next line
        }
        else
        {
            EndDialogue();  // No more lines left
        }
    }

    // Ends the current dialogue
    private void EndDialogue()
    {
        // If there's a chained dialogue, start it
        if (currentDialogue.nextDialogue != null)
        {
            StartDialogue(currentDialogue.nextDialogue);
            return;
        }

        // Otherwise, close the dialogue
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    // Space key to advance dialogue
    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ContinueDialogue();
        }
    }
}