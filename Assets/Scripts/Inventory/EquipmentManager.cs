using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character character;
    [SerializeField] private Inventory inventory;

    [Header("Equipment Slots Mapping")]
    [SerializeField] private List<EquipmentSlotUI> equipmentSlots = new List<EquipmentSlotUI>();

    private void Awake()
    {
        // 프로젝트 전반에 장비창이 아예 없다면 이 컴포넌트를 오브젝트에서 떼거나 끄면 됨.
        InitializeStartingEquipments();
    }

    private void OnEnable()
    {
        // 전역 슬롯 이벤트를 구독하여 장비 슬롯에 대한 동작을 가로채서 독립 처리
        BaseItemSlotUI.OnSlotRightClickEvent += HandleEquipmentRightClick;
        BaseItemSlotUI.OnSlotDropEvent += HandleEquipmentDrop;
    }

    private void OnDisable()
    {
        BaseItemSlotUI.OnSlotRightClickEvent -= HandleEquipmentRightClick;
        BaseItemSlotUI.OnSlotDropEvent -= HandleEquipmentDrop;
    }

    private void InitializeStartingEquipments()
    {
        if (equipmentSlots == null) return;
        foreach (var slotUI in equipmentSlots)
        {
            if (slotUI != null && slotUI.Slot != null && slotUI.Slot.Item is EquippableItem equippable)
            {
                equippable.Equip(character);
            }
        }
    }

    // 슬롯 교환(드롭) 시 장비 스탯 버프의 적용 및 해제 제어
    private void HandleEquipmentDrop(BaseItemSlotUI dropSlotUI)
    {
        // 무언가 스왑이 일어날 때, 관여된 슬롯들 중 EquipmentSlotUI가 있다면 스탯 연산 수행
        // 단, InventoryController가 데이터 변경(수왑)하기 전에 감지되므로 순서에 맞춰 버프 변경 적용
        // 가장 직관적인 방법은 슬롯 변경 알림(OnSlotChanged)을 추적하는 것입니다.
    }

    private void HandleEquipmentRightClick(BaseItemSlotUI slotUI)
    {
        Item item = slotUI.Slot.Item;
        if (item == null) return;

        // 1. 장비 슬롯 자체를 우클릭한 경우 -> 해제하여 인벤토리로 이동
        if (slotUI is EquipmentSlotUI equipSlot)
        {
            UnequipViaRightClick(equipSlot);
            return;
        }

        // 2. 일반 일반/퀵슬롯에서 장비 아이템을 우클릭한 경우 -> 알맞은 장비 슬롯으로 자동 장착
        if (item is EquippableItem equippable)
        {
            EquipViaRightClick(slotUI, equippable);
        }
    }

    private void EquipViaRightClick(BaseItemSlotUI sourceSlot, EquippableItem item)
    {
        if (equipmentSlots == null) return;

        foreach (var targetSlot in equipmentSlots)
        {
            if (targetSlot != null && targetSlot.SlotType == item.EquipmentType)
            {
                if (targetSlot.Slot.Item is EquippableItem oldEquip) oldEquip.Unequip(character);
                item.Equip(character);

                Item temp = targetSlot.Slot.Item;
                targetSlot.Slot.Item = item;
                sourceSlot.Slot.Item = temp;

                targetSlot.Slot.UpdateSlot();
                sourceSlot.Slot.UpdateSlot();
                return;
            }
        }
    }

    private void UnequipViaRightClick(EquipmentSlotUI sourceSlot)
    {
        if (inventory == null) return;

        EquippableItem item = sourceSlot.Slot.Item as EquippableItem;
        if (item == null) return;

        if (inventory.AddItemCustomPriority(item, 1))
        {
            item.Unequip(character);
            sourceSlot.Slot.Item = null;
            sourceSlot.Slot.Amount = 0;
            sourceSlot.Slot.UpdateSlot();
        }
    }
}