/*
 * 
 * controls the dynamic rendering of a ClassPanel element based on inputted class
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class ClassSelectPanel : MonoBehaviour
{   
    // UI gameobject links
    [SerializeField] TextMeshProUGUI classNameText;
    [SerializeField] TextMeshProUGUI classDescriptionText;
    [SerializeField] TextMeshProUGUI classStatsText;
    [SerializeField] Image classPreviewImage;

    private Class currentClass;

    public delegate void OnClickDelegate(Class c);
    public event OnClickDelegate OnClickEvent;

    // takes a class as input and shows it
    public void SetDisplayClass (Class show)
    {
        classNameText.SetText(show.className);
        classDescriptionText.SetText(show.description);
        classPreviewImage.sprite = show.previewSprite;
        classStatsText.SetText("Health: " + show.maxHealth + "\nSpeed: " + show.moveSpeed);
        currentClass = show;

        Debug.Log("now showing info for class " + show);
    }

    // get the current display class
    public Class GetDisplayClass()
    {
        return currentClass;
    }

    // when the panel is clicked
    public void OnClick()
    {
        if (OnClickEvent != null)   
            OnClickEvent(currentClass);
    }
}
