/*
 * 
 * contains fixed property data for items to be referenced from in the itemdex
 * TODO: make an abstract class, so items can be finely categorized
 * 
 */

using UnityEngine;

public class ItemData : MonoBehaviour
{
    // fields here are public and mutable
    // do not mutate these values
    public string id;

    // DATA FIELDS
    public string displayName;
    public int maxStackSize;
    public Sprite sprite;

    public override string ToString()
    {
        return id + "_itemdata";
    }
}

