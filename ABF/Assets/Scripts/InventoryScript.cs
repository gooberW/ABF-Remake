using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    [SerializeField] private GameObject hotbar;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource source;

    private float SLOT_HEIGHT = 60f;
    private float SEL_SLOT_HEIGHT = 100f;
    private float INIT_OPACITY = 80f / 255f;
    private float SEL_OPACITY = 128f / 255f;
    private int NUM_SLOTS = 3;

    private List<Image> slotImages = new List<Image>();
    private CanvasGroup hotbarGroup;
    private Coroutine hideCoroutine;


    [SerializeField] private float visibleDuration = 2f; // quanto tempo demora ate a hotbar desaparecer depois de escolher

    void Start()
    {
        hotbarGroup = hotbar.GetComponent<CanvasGroup>();
        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 0;
            hotbarGroup.interactable = false;
            hotbarGroup.blocksRaycasts = false;
        }

        for (int i = 1; i <= NUM_SLOTS; i++)
        {
            GameObject slotObj = GameObject.FindWithTag("Slot" + i);
            if (slotObj != null)
            {
                Image img = slotObj.GetComponent<Image>();
                if (img != null)
                {
                    slotImages.Add(img);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlotWithTag("Slot1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlotWithTag("Slot2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlotWithTag("Slot3");
        }
    }

    void SelectSlotWithTag(string tag)
    {
        GameObject selectedObj = GameObject.FindWithTag(tag);
        if (selectedObj != null)
        {
            Image selectedImage = selectedObj.GetComponent<Image>();
            if (selectedImage != null)
            {
                source.PlayOneShot(sound);
                ShowHotbar();
                SelectSlot(selectedImage);

                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                }
                hideCoroutine = StartCoroutine(HideHotbarAfterDelay(visibleDuration));
            }
        }
    }


    void ShowHotbar()
    {
        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 1;
            hotbarGroup.interactable = true;
            hotbarGroup.blocksRaycasts = true;
        }
    }

    IEnumerator HideHotbarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 0;
            hotbarGroup.interactable = false;
            hotbarGroup.blocksRaycasts = false;
        }
    }

    void SelectSlot(Image selected)
    {
        foreach (Image slot in slotImages)
        {
            RectTransform rt = slot.GetComponent<RectTransform>();
            bool isSelected = (slot == selected);

            rt.sizeDelta = new Vector2(rt.sizeDelta.x, isSelected ? SEL_SLOT_HEIGHT : SLOT_HEIGHT);

            Color slotColor = slot.color;

            if (isSelected)
            {
                slotColor.a = SEL_OPACITY;
            }
            else
            {
                slotColor.a = INIT_OPACITY;
            }

            slot.color = slotColor;

            TextMeshProUGUI number = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (number != null)
            {
                Color textColor = number.color;
                textColor.a = slotColor.a;
                number.color = textColor;
            }
        }
    }
}
