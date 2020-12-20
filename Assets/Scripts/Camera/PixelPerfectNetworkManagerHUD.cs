using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PixelPerfectNetworkManagerHUD : NetworkManagerHUD
{
    public Vector2 customOffset;

    Vector2 lastSize;

    public override void Awake()
    {
        base.Awake();
        lastSize = Camera.main.rect.size;
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (Camera.main.rect.size != lastSize)
        {
            int adjustX = (int) ((((1 - Camera.main.rect.size.x) / 2) * Screen.width) + customOffset.x);
            int adjustY = (int) ((((1 - Camera.main.rect.size.y) / 2) * Screen.height) + customOffset.y);
            Debug.Log("camera offset is now: " + adjustX + "," + adjustY);
            offsetX = adjustX;
            offsetY = adjustY;

            lastSize = Camera.main.rect.size;
        }
    }
}
