using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("First button to be selected when this menu opens")]
    public GameObject firstSelected;

    private void OnEnable()
    {
        // Reset and set selection to ensure EventSystem recognizes it
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
