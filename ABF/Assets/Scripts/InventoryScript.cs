using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private float visibleDuration = 2f;

    [Header("UI References")]
    [SerializeField] private GameObject hotbar;
    [SerializeField] private Image[] slotImages = new Image[3];
    [SerializeField] private Image[] slotIcons = new Image[3];
    [SerializeField] private TextMeshProUGUI[] itemNameTexts = new TextMeshProUGUI[3];

    [Header("Audio")]
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource source;

    private const float SLOT_HEIGHT = 60f;
    private const float SEL_SLOT_HEIGHT = 80f;
    private const float INIT_OPACITY = 80f / 255f;
    private const float SEL_OPACITY = 128f / 255f;

    private CanvasGroup hotbarGroup;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        hotbarGroup = hotbar.GetComponent<CanvasGroup>();
        InitializeUI();
        inventoryData.Clear();
    }

    private void InitializeUI()
    {
        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 0;
            hotbarGroup.interactable = false;
            hotbarGroup.blocksRaycasts = false;
        }

        for (int i = 0; i < 3; i++)
        {
            UpdateSlotUI(i);
        }
    }

    public void AddItem(GameObject item, Sprite sprite, string itemName)
    {
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            if (inventoryData.slots[i].IsEmpty)
            {
                inventoryData.slots[i] = new InventoryData.InventorySlot
                {
                    itemObject = item,
                    itemSprite = sprite,
                    itemName = itemName
                };

                item.SetActive(i == inventoryData.currentSelectedSlot);
                UpdateSlotUI(i);

                if (inventoryData.currentSelectedSlot == -1)
                {
                    SelectSlot(i);
                }

                return;
            }
        }
        Debug.LogWarning("Inventory is full!");
    }

    public GameObject GetCurrentItem()
    {
        if (inventoryData.currentSelectedSlot >= 0 &&
            !inventoryData.slots[inventoryData.currentSelectedSlot].IsEmpty)
        {
            return inventoryData.slots[inventoryData.currentSelectedSlot].itemObject;
        }
        return null;
    }

    public void RemoveItem(GameObject itemToRemove)
    {
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            if (!inventoryData.slots[i].IsEmpty &&
                inventoryData.slots[i].itemObject == itemToRemove)
            {
                inventoryData.slots[i] = new InventoryData.InventorySlot();
                UpdateSlotUI(i);

                if (inventoryData.currentSelectedSlot == i)
                {
                    inventoryData.currentSelectedSlot = -1;
                    DeactivateAllItems();
                }
                return;
            }
        }
    }

    private void Update()
    {
        HandleHotkeyInput();
    }

    private void HandleHotkeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= inventoryData.slots.Length) return;

        inventoryData.currentSelectedSlot = index;
        UpdateAllSlotsUI();

        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            if (!inventoryData.slots[i].IsEmpty)
            {
                inventoryData.slots[i].itemObject.SetActive(i == index);
            }
        }

        ShowHotbarWithFeedback();
    }

    private void ShowHotbarWithFeedback()
    {
        source.PlayOneShot(sound);
        ShowHotbar();

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideHotbarAfterDelay(visibleDuration));
    }

    private void UpdateSlotUI(int index)
    {
        bool hasItem = !inventoryData.slots[index].IsEmpty;
        bool isSelected = inventoryData.currentSelectedSlot == index;

        // Update slot appearance
        if (slotImages[index] != null)
        {
            RectTransform rt = slotImages[index].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, isSelected ? SEL_SLOT_HEIGHT : SLOT_HEIGHT);

            Color slotColor = slotImages[index].color;
            slotColor.a = isSelected ? SEL_OPACITY : INIT_OPACITY;
            slotImages[index].color = slotColor;
        }

        // Update icon
        if (slotIcons[index] != null)
        {
            slotIcons[index].enabled = hasItem;
            if (hasItem)
            {
                slotIcons[index].sprite = inventoryData.slots[index].itemSprite;
                slotIcons[index].color = new Color(1, 1, 1, slotImages[index].color.a);
            }
        }

        // Update name
        if (itemNameTexts[index] != null)
        {
            itemNameTexts[index].text = hasItem && isSelected ? inventoryData.slots[index].itemName : "";
            itemNameTexts[index].color = new Color(1, 1, 1, isSelected ? 1f : 0f);
        }
    }

    private void UpdateAllSlotsUI()
    {
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }

    private void DeactivateAllItems()
    {
        foreach (var slot in inventoryData.slots)
        {
            if (!slot.IsEmpty)
            {
                slot.itemObject.SetActive(false);
            }
        }
    }

    private void ShowHotbar()
    {
        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 1;
            hotbarGroup.interactable = true;
            hotbarGroup.blocksRaycasts = true;
        }
    }

    private IEnumerator HideHotbarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 0;
            hotbarGroup.interactable = false;
            hotbarGroup.blocksRaycasts = false;
        }
    }
}
