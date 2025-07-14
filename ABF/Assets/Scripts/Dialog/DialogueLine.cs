using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName;  // The name of the character speaking
    [TextArea(3, 5)] public string text;  // The dialogue text
}