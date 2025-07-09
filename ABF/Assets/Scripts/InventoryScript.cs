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
    private float SEL_SLOT_HEIGHT = 80f;
    private float INIT_OPACITY = 80f / 255f;
    private float SEL_OPACITY = 128f / 255f;
    private int NUM_SLOTS = 3;

    private List<Image> slotImages = new List<Image>();
    private List<Image> slotIcons = new List<Image>();
    private List<TextMeshProUGUI> itemNameTexts = new List<TextMeshProUGUI>();
    private List<string> itemNames = new List<string>();
    [SerializeField] private List<GameObject> ItemsInHand = new List<GameObject>();

    private CanvasGroup hotbarGroup;
    private Coroutine hideCoroutine;
    private int currentSelectedSlot = -1;

    [SerializeField] private float visibleDuration = 2f;

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
                Image slotImg = slotObj.GetComponent<Image>();
                if (slotImg != null)
                    slotImages.Add(slotImg);

                // Get second child (icon)
                if (slotObj.transform.childCount >= 2)
                {
                    Image icon = slotObj.transform.GetChild(1).GetComponent<Image>();
                    slotIcons.Add(icon);
                }
                else
                {
                    slotIcons.Add(null);
                }

                // Get third child (item name)
                if (slotObj.transform.childCount >= 3)
                {
                    TextMeshProUGUI nameText = slotObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    itemNameTexts.Add(nameText);
                }
                else
                {
                    itemNameTexts.Add(null);
                }
            }
        }

        // Initialize all icons and names as hidden
        for (int i = 0; i < slotIcons.Count; i++)
        {
            if (slotIcons[i] != null)
            {
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }

            if (itemNameTexts[i] != null)
            {
                itemNameTexts[i].text = "";
            }

            itemNames.Add(""); // Fill itemNames with empty entries
        }
    }

    public void AddItem(GameObject item, Sprite sprite, string itemName)
    {
        if (ItemsInHand.Count >= slotIcons.Count)
            return;

        ItemsInHand.Add(item);
        item.SetActive(ItemsInHand.Count == 1);

        int index = ItemsInHand.Count - 1;

        if (slotIcons[index] != null)
        {
            slotIcons[index].sprite = sprite;
            slotIcons[index].enabled = true;
        }

        itemNames[index] = itemName;

        if (itemNameTexts[index] != null)
        {
            itemNameTexts[index].text = "";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlotWithTag("Slot1");
            SetActiveItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlotWithTag("Slot2");
            SetActiveItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlotWithTag("Slot3");
            SetActiveItem(2);
        }
    }

    void SetActiveItem(int index)
    {
        foreach (var item in ItemsInHand)
        {
            if (item != null)
                item.SetActive(false);
        }

        if (index >= 0 && index < ItemsInHand.Count && ItemsInHand[index] != null)
        {
            ItemsInHand[index].SetActive(true);
        }

        currentSelectedSlot = index;
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
                    StopCoroutine(hideCoroutine);

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
        for (int i = 0; i < slotImages.Count; i++)
        {
            Image slot = slotImages[i];
            RectTransform rt = slot.GetComponent<RectTransform>();
            bool isSelected = (slot == selected);

            rt.sizeDelta = new Vector2(rt.sizeDelta.x, isSelected ? SEL_SLOT_HEIGHT : SLOT_HEIGHT);

            Color slotColor = slot.color;
            slotColor.a = isSelected ? SEL_OPACITY : INIT_OPACITY;
            slot.color = slotColor;

            TextMeshProUGUI number = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (number != null)
            {
                Color textColor = number.color;
                textColor.a = slotColor.a;
                number.color = textColor;
            }

            // Fade icon
            if (slotIcons[i] != null)
            {
                Color iconColor = slotIcons[i].color;
                iconColor.a = slotColor.a;
                slotIcons[i].color = iconColor;
            }

            // Set item name
            if (itemNameTexts[i] != null)
            {
                if (isSelected && i < itemNames.Count)
                {
                    itemNameTexts[i].text = itemNames[i];
                    Color nameColor = itemNameTexts[i].color;
                    nameColor.a = 1f;
                    itemNameTexts[i].color = nameColor;
                }
                else
                {
                    itemNameTexts[i].text = "";
                }
            }
        }
    }
}
