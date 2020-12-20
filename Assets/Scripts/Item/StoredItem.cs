/*
 * items as they exist in storage, can represent stacks of items
 */

using UnityEngine;

public class StoredItem
{
    public readonly string id;
    private readonly int maxStackSize;
    private int stackSize;

    public StoredItem(){}

    public StoredItem(string _id, int _stackSize)
    {
        id = _id;
        maxStackSize = Itemdex.Instance.GetItemData(id).maxStackSize;
        if (_stackSize < maxStackSize && _stackSize > 0)
        {
            stackSize = _stackSize;
        }
        else
        {
            stackSize = maxStackSize;
            Debug.Log("tried adding illegal amount of " + id + " to a new stack (" + _stackSize + "/" + maxStackSize + ")");
        }
    }

    // converts an entity item to a stored item
    public static StoredItem EntityItemToStoredItem(EntityItem i)
    {
        return new StoredItem(i.id, 1);
    }

    // sanity checks for adding to stacks
    bool CanAddStoredItems(StoredItem i)
    {
        if (id.Equals(i.id)) {
            if (stackSize + i.stackSize > maxStackSize)
            {
                return true;
            }
        }
        return false;
    }

    void TryAddStoredItems(StoredItem i)
    {
        if (CanAddStoredItems(i))
        {
            stackSize += i.stackSize;
        }
    }

    public override string ToString()
    {
        return id + " (" + stackSize + "/" + maxStackSize + ")";
    }
}
