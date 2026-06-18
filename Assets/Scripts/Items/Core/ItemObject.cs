using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public static event Action<Item> OnAnyItemPickedUp;

    [SerializeField] private Item item;
    [SerializeField] private int amount = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnValidate() => UpdateSprite();
    private void Awake() => UpdateSprite();

    private void UpdateSprite()
    {
        if (item != null && spriteRenderer != null)
            spriteRenderer.sprite = item.Icon;
    }

    public Item GetItem() => item;
    public int GetAmount() => amount;

    public void OnPickedUp()
    {

        if (item != null)
        {
            OnAnyItemPickedUp?.Invoke(item);
        }

        Destroy(gameObject);
    }

    public void SetItemData(Item newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
        UpdateSprite();
    }
}