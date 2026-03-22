public interface IItemContainer
{
    bool AddItem(Item item);
    bool RemoveItemByID(string itemID);
    bool IsFull();
    int ItemCount(string itemID);
}