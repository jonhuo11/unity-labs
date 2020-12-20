using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Class : MonoBehaviour
{
    [Header("Class descriptions")]
    public string className;
    [TextArea] public string description;
    public Sprite previewSprite;

    [Header("Class stats")]
    public int maxHealth;
    public float moveSpeed;

    public override string ToString()
    {
        return className;
    }
}
