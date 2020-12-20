/*
 * 
 * global registry of all items in the game and their properties
 * loaded in from a json/castledb file before the game launches
 * 
 * (string, ItemData) hashmap of string to itemdata
 * items in game just contain a reference to the itemId in the Itemdex, properties are looked up on use
 * 
 * when a new lobby is hosted:
 *  - server spawns the itemdex instance
 *  - server spawns the gamemanager instance
 * 
 * EntityItem: physical item that can be dropped on the floor, the instantiated item, has reference to Itemdex id
 * StoredItem: only exists in arrays/storage, when dropped in real world it is converted to EntityItem
 * ItemData: simple container for item properties
 * RegistryItem: prefab, has all information needed to spawn an item, however it is only a template that is never instantiated
 * 
 */

using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Itemdex : NetworkBehaviour
{
    private static Itemdex _instance;
    public static Itemdex Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("itemdex not initialized");
            }
            return _instance;
        }
    }

    private Dictionary<string, ItemData> registry = new Dictionary<string, ItemData>();
    // TODO: load items from a static castledb or json
    // array of itemdata that can be edited in the inspector
    [SerializeField] private ItemData[] tempItemList;

    // retrieves the data for an item from the registry
    public ItemData GetItemData(string key)
    {
        if (registry.ContainsKey(key))
        {
            return registry[key];
        }
        Debug.LogError("no item with id " + key + " was found in the itemdex");
        return null;
    }

    [Server]
    void LoadRegistry()
    {
        foreach (ItemData i in tempItemList)
        {
            registry.Add(i.id, i);
        }
        Debug.Log("itemdex registry loaded");
        PrintRegistry();
    }

    [Server]
    void PrintRegistry()
    {
        string output = "";
        foreach (KeyValuePair<string, ItemData> pair in registry)
        {
            output += "(" + pair.Key + "," + pair.Value + ") ";
        }
        Debug.Log(output);
    }

    [Server]
    void Awake()
    {
        _instance = this;

        LoadRegistry();
    }
}
