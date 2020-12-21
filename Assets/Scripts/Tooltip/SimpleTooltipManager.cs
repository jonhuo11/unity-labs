/*
 * 
 * singleton tooltip manager for drawing tooltips on the screen
 * 
 * list of tooltip prefabs
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTooltipManager : MonoBehaviour
{
    public static SimpleTooltipManager Instance { get; private set; }

    public void ShowTooltip(SimpleTooltipTrigger t)
    {
        try
        {
            SimpleTooltip.Instance.gameObject.SetActive(true);
            SimpleTooltip.Instance.UpdateTooltip(t);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("SimpleTooltipManager requires a single gameobject with script SimpleTooltip attached to be present in the scene\n" + e);
        }
    }

    public void HideTooltip()
    {
        try
        {
            SimpleTooltip.Instance.gameObject.SetActive(false);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("SimpleTooltipManager requires a single gameobject with script SimpleTooltip attached to be present in the scene\n" + e);
        }
    }

    void Start()
    {
        Instance.HideTooltip();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("found existing instance of TooltipManager, deleting this instance");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

}
