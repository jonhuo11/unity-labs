using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectMenu : MonoBehaviour
{
    [SerializeField] ChoiceGroup teamChoicesGroup;
    [SerializeField] Button confirmButton;
    [SerializeField] AudioClip clickSound;

    AudioSource audioSrc;

    // called when the confirm button is pressed
    void OnConfirm()
    {
        Choice c = teamChoicesGroup.GetChosen();
        if (c != null) // final sanity check
        {
            // TODO: do something (tell GameManager to update the UI being shown)
            audioSrc.Play();
            Debug.Log(c + " was confirmed as the chosen team");
        }
    }

    // called when a choice is made
    void OnChoiceChanged()
    {
        Choice c = teamChoicesGroup.GetChosen();
        if (c != null)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
        audioSrc.Play();
    }

    void OnEnable()
    {
        if (teamChoicesGroup != null)
        {
            teamChoicesGroup.OnChoiceChangedEvent += OnChoiceChanged;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void OnDisable()
    {
        if (teamChoicesGroup != null)
        {
            teamChoicesGroup.OnChoiceChangedEvent -= OnChoiceChanged;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirm);
        }
    }

    void Start()
    {
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.clip = clickSound;
    }

}
