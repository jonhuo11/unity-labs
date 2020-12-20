/*
 * parent class for storages
 * represents an inventory in the game, essentially a grid of StoredItem (item stacks)
 * 
 * client does not have to know what is inside a chest until they open it?
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Storage : NetworkBehaviour
{
    // TODO: make into a grid, for now we can have a list
    // TODO: clients should only download this list if they open the chest
    private SyncList<StoredItem> items = new SyncList<StoredItem>();
    [SyncVar] private bool inUse; // clients cannot use a storage if it is already in use

    public void AddItem(StoredItem i)
    {
        items.Add(i);
    }
}
