// InventoryData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/Inventory Data")]
public class InventoryData : ScriptableObject
{
    [System.Serializable]
    public class InventorySlot
    {
        public GameObject itemObject;
        public Sprite itemSprite;
        public string itemName;
        public bool IsEmpty => itemObject == null;
    }

    public InventorySlot[] slots = new InventorySlot[3];
    public int currentSelectedSlot = -1;

    public void Clear()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot();
        }
        currentSelectedSlot = -1;
    }
}