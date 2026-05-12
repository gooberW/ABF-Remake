using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Behaviour")]
    [SerializeField] private bool blockPlayerMovement = true;

    [Header("Audio")]
    [SerializeField] private AudioSource defaultAudioSource;

    [Header("Events")]
    public UnityEvent onDialogueEnd;

    [System.Serializable]
    public class NamedAudioSource
    {
        public string key;
        public AudioSource source;
    }

    [Header("Named Audio Sources")]
    [SerializeField] private NamedAudioSource[] namedAudioSources;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private bool isDialogueActive;
    private Coroutine typingCoroutine;
    private bool isTyping;
    private UnityEvent onCompleteCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (defaultAudioSource == null)
        {
            defaultAudioSource = GetComponent<AudioSource>();
            if (defaultAudioSource == null)
                defaultAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void StartDialogue(Dialogue dialogue, UnityEvent onComplete = null)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        onCompleteCallback = onComplete;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (blockPlayerMovement) PlayerScript.CanMove = false;
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Length) return;

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        if (characterNameText != null) characterNameText.text = line.characterName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(TypeLine(line.text));

        AudioSource src = GetAudioSourceForKey(line.audioSourceKey) ?? defaultAudioSource;

        if (line.voiceClip != null && src != null)
        {
            src.Stop();
            src.clip = line.voiceClip;
            src.Play();
        }
        else if (src != null && src.isPlaying && line.voiceClip == null)
        {
            src.Stop();
        }
    }

    private AudioSource GetAudioSourceForKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        if (namedAudioSources != null)
        {
            for (int i = 0; i < namedAudioSources.Length; i++)
            {
                if (namedAudioSources[i] == null) continue;
                if (string.Equals(namedAudioSources[i].key, key, System.StringComparison.OrdinalIgnoreCase))
                    return namedAudioSources[i].source;
            }
        }
        return null;
    }

    private System.Collections.IEnumerator TypeLine(string text)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (dialogueText != null) dialogueText.text += text[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    public void ContinueDialogue()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            DialogueLine currentLine = currentDialogue.lines[currentLineIndex];
            if (dialogueText != null) dialogueText.text = currentLine.text;
            isTyping = false;
            return;
        }

        currentLineIndex++;

        if (currentDialogue != null && currentLineIndex < currentDialogue.lines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (currentDialogue != null && currentDialogue.nextDialogue != null)
        {
            StartDialogue(currentDialogue.nextDialogue, onCompleteCallback); 
            return;
        }

        isDialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (blockPlayerMovement) PlayerScript.CanMove = true;
        if (defaultAudioSource != null && defaultAudioSource.isPlaying) defaultAudioSource.Stop();

        onCompleteCallback?.Invoke(); 
        onCompleteCallback = null;
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) ContinueDialogue();
    }
}