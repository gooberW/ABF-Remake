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

    public static bool isInvFull = false;

    private void Awake()
    {
        hotbarGroup = hotbar.GetComponent<CanvasGroup>();
        inventoryData.Clear();
        InitializeUI();
    }

    private void Start()
    {
        HideHotbar();
    }

    private void InitializeUI()
    {
        HideHotbar();
        UpdateAllSlotsUI();
    }

    private void HideHotbar()
    {
        if (hotbarGroup != null)
        {
            hotbarGroup.alpha = 0;
            hotbarGroup.interactable = false;
            hotbarGroup.blocksRaycasts = false;
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
        int idx = inventoryData.currentSelectedSlot;
        if (idx >= 0 && idx < inventoryData.slots.Length && !inventoryData.slots[idx].IsEmpty)
        {
            return inventoryData.slots[idx].itemObject;
        }
        return null;
    }

    public void RemoveItem(GameObject itemToRemove)
    {
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];
            if (!slot.IsEmpty && slot.itemObject == itemToRemove)
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
        isInvFull = IsInventoryFull();
    }

    public bool IsInventoryFull()
    {
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            if (inventoryData.slots[i].IsEmpty)
                return false;
        }
        return true;
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
        if (inventoryData.currentSelectedSlot == index) return;

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
        if (sound != null && source != null)
            source.PlayOneShot(sound);

        ShowHotbar();

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideHotbarAfterDelay(visibleDuration));
    }

    private void UpdateSlotUI(int index)
    {
        var slot = inventoryData.slots[index];
        bool hasItem = !slot.IsEmpty;
        bool isSelected = inventoryData.currentSelectedSlot == index;

        if (slotImages[index] != null)
        {
            RectTransform rt = slotImages[index].rectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, isSelected ? SEL_SLOT_HEIGHT : SLOT_HEIGHT);

            Color slotColor = slotImages[index].color;
            slotColor.a = isSelected ? SEL_OPACITY : INIT_OPACITY;
            slotImages[index].color = slotColor;
        }

        if (slotIcons[index] != null)
        {
            slotIcons[index].enabled = hasItem;
            if (hasItem)
            {
                slotIcons[index].sprite = slot.itemSprite;
                slotIcons[index].color = new Color(1, 1, 1, slotImages[index].color.a);
            }
        }

        if (itemNameTexts[index] != null)
        {
            itemNameTexts[index].text = (hasItem && isSelected) ? slot.itemName : "";
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
            if (!slot.IsEmpty && slot.itemObject != null)
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
        HideHotbar();
    }
}