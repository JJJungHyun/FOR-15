using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    [SerializeField] private Furnace furnace;
    [SerializeField] private BaseItemSlotUI inputSlotUI;
    [SerializeField] private BaseItemSlotUI outputSlotUI;
    [SerializeField] private Image ghostIcon; // 흐릿하게 보일 아이콘

    private void Start()
    {
        // 데이터와 UI 연결
        inputSlotUI.SetSlot(furnace.itemSlots[0]);
        outputSlotUI.SetSlot(furnace.itemSlots[1]);
        ghostIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        float progress = furnace.GetProgress();
        var recipe = furnace.GetCurrentRecipe();

        // 1. 결과 슬롯에 실제 템이 없을 때만 고스트 아이콘 표시
        if (furnace.IsCooking && recipe != null && furnace.itemSlots[1].Item == null)
        {
            ghostIcon.gameObject.SetActive(true);
            ghostIcon.sprite = recipe.OutputItem.Icon;

            // 2. 투명도를 0.2에서 1.0으로 서서히 변경 (선명해지는 효과)
            Color c = ghostIcon.color;
            c.a = Mathf.Lerp(0.2f, 1.0f, progress);
            ghostIcon.color = c;
        }
        else
        {
            ghostIcon.gameObject.SetActive(false);
        }
    }
}