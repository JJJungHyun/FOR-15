public interface IItemContainer
{
    bool AddItem(Item item);
    bool RemoveItem(Item item);
    bool IsFull();
    int ItemCount(string itemID);
}