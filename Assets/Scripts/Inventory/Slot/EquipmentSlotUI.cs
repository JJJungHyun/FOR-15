using UnityEngine;

public class EquipmentSlotUI : BaseItemSlotUI
{
    [Header("Equipment Settings")]
    public EquipmentType SlotType;

    private void Awake()
    {
        if (_slot == null)
        {
            SetSlot(new ItemSlot());
        }
    }

    public override bool CanReceiveItem(Item item)
    {
        if (item == null) return true;

        if (item is EquippableItem equippable)
        {
            return equippable.EquipmentType == SlotType;
        }
        return false;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = SlotType.ToString() + " Slot";
    }
#endif
}