/*
 *  attach script to gameobjects that trigger tooltips when hovered over
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [TextArea] public string content;

    IEnumerator activePopupRoutine;

    public void OnPointerEnter(PointerEventData data)
    {
        if (activePopupRoutine != null)
        {
            StopCoroutine(activePopupRoutine);
            Debug.Log("existing popup routine detected and deleted before spawning new routine");
        }
        activePopupRoutine = PopupTooltip();
        StartCoroutine(activePopupRoutine);
        Debug.Log("starting popup routine");
    }

    public void OnPointerExit(PointerEventData data)
    {
        if(activePopupRoutine != null)
        {
            StopCoroutine(activePopupRoutine);
            Debug.Log("stopping popup routine");
        }
        SimpleTooltipManager.Instance.HideTooltip();
    }


    // delayed popup in case the user is just quickly clicking
    IEnumerator PopupTooltip()
    {
        yield return new WaitForSeconds(SimpleTooltip.Instance.popupAfterSeconds);
        SimpleTooltipManager.Instance.ShowTooltip(this);
        Debug.Log("popped up!");
        yield break;
    }
}
