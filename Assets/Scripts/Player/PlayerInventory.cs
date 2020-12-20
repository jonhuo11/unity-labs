using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Storage
{
    public float pickupRange;

    void Update()
    {
        Physics2D.OverlapCircle(transform.position, pickupRange);
    }
}
