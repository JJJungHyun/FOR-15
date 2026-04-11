using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHUD : MonoBehaviour
{
    [SerializeField] private CharConditionHandler conditionHandler;
    [SerializeField] private GameObject effectSlotPrefab;
    [SerializeField] private Transform container; 

    private List<StatusEffectSlotUI> activeSlots = new List<StatusEffectSlotUI>();

    private void OnEnable()
    {

    }

    private void Update()
    {
        SyncUI(conditionHandler.GetActiveEffects());
    }

    private void SyncUI(List<ConditionEffect> currentEffects)
    {
        foreach (var effect in currentEffects)
        {
            if (!activeSlots.Exists(s => s != null && s.EffectName == effect.Name))
            {
                var newSlot = Instantiate(effectSlotPrefab, container).GetComponent<StatusEffectSlotUI>();
                newSlot.Setup(effect);
                activeSlots.Add(newSlot);
            }
        }

        activeSlots.RemoveAll(s => s == null);
    }
}