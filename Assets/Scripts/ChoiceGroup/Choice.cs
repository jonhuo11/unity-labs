/*
 * attach to gameobject that you want to be a part of a ChoiceGroup
 * use group field in inspector to link to the ChoiceGroup the object belongs to
 * must be a UI gameobject containing image component
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Choice : 
    MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public ChoiceGroup group;

    [Header("Customizable colors")]
    Color defaultColor; // color of the imagecomponent
    [SerializeField] Color chosenColor;

    // choice events for the ChoiceGroup to subscribe to
    public delegate void OnChosenDelegate(Choice c);
    public event OnChosenDelegate OnChosenEvent;

    // components
    Image image;

    public void OnPointerEnter(PointerEventData data)
    {
        image.color = chosenColor;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (IsChosen())
        {
            image.color = chosenColor;
        }
        else
        {
            image.color = defaultColor;
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        OnChosenEvent(this);
    }

    void OnChoiceChanged()
    {
        if (IsChosen())
        {
            image.color = chosenColor;
        }
        else
        {
            image.color = defaultColor;
        }
    }

    bool IsChosen()
    {
        if (group != null)
        {
            return group.IsChosen(this);
        }
        return false;
    }

    void OnEnable()
    {
        if (group != null)
        {
            group.AddChoice(this);
            group.OnChoiceChangedEvent += OnChoiceChanged;
        }
    }

    void OnDisable()
    {
        if (group != null)
        {
            group.RemoveChoice(this);
            group.OnChoiceChangedEvent -= OnChoiceChanged;
        }
    }

    void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

}
