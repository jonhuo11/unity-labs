/*
 * add to gameobject that manages a choice group
 * when a new choice is chosen, it has a event that fires
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceGroup : MonoBehaviour
{
    private HashSet<Choice> choices = new HashSet<Choice>();
    private Choice chosen;

    public delegate void OnChoiceChangedDelegate();
    public event OnChoiceChangedDelegate OnChoiceChangedEvent;

    void ChangeChoice(Choice c)
    {
        // ChoiceGroup controls the display settings of each choice
        if (choices.Contains(c) && chosen != c)
        {
            chosen = c;
            OnChoiceChangedEvent();
            Debug.Log("changed chosen to " + c);
        }
        else if (chosen == c) // deselect
        {
            chosen = null;
            OnChoiceChangedEvent();
            Debug.Log("deselected choice " + c + ", currently there are no choices selected");
        }
        else
        {
            Debug.LogError("choice " + c + " does not exist in set of choices");
        }
    }

    public Choice GetChosen()
    {
        return chosen;
    }

    public bool IsChosen(Choice c)
    {
        return c == chosen && choices.Contains(chosen);
    }

    public bool AddChoice(Choice c)
    {
        c.OnChosenEvent += ChangeChoice;
        return choices.Add(c);
    }

    public bool RemoveChoice(Choice c)
    {
        c.OnChosenEvent -= ChangeChoice;
        return choices.Remove(c);
    }
}