/*
 * items as they exist in drop form, can be picked up by player and moved by physics
 */

using Mirror;

public class EntityItem : NetworkBehaviour
{
    public readonly string id;

    EntityItem(string id)
    {
        this.id = id;
    }

    ItemData GetItemData()
    {
        return Itemdex.Instance.GetItemData(id);
    }

    string GetDisplayName()
    {
        return GetItemData().displayName;
    }
}
