using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    [SerializeField] private Furnace furnace;
    [SerializeField] private BaseItemSlotUI inputSlotUI;
    [SerializeField] private BaseItemSlotUI outputSlotUI;
    [SerializeField] private Image ghostIcon; 

    private void Start()
    {
        inputSlotUI.SetSlot(furnace.itemSlots[0]);
        outputSlotUI.SetSlot(furnace.itemSlots[1]);
        ghostIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        float progress = furnace.GetProgress();
        var recipe = furnace.GetCurrentRecipe();

        if (furnace.IsCooking && recipe != null && furnace.itemSlots[1].Item == null)
        {
            ghostIcon.gameObject.SetActive(true);
            ghostIcon.sprite = recipe.OutputItem.Icon;

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