using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectRoomTypePanel : MonoBehaviour
{
    [SerializeField] private GameObject previousPanel;
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private GameObject enterPanel;
    [SerializeField] private Button makeRoomButton;
    
    private bool isRequesting = false;
    
    public void OnMakeRoomButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        if (isRequesting) return;

        isRequesting = true;
        makeRoomButton.interactable = false;
        
        Server.Instance.StartGame(GameMode.Host);
    }

    public void OnEnterRoomButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        currentPanel.SetActive(false);
        enterPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
