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
        if (isRequesting) return;

        isRequesting = true;
        makeRoomButton.interactable = false;
        
        Server.Instance.StartGame(GameMode.Host);
    }

    public void OnEnterRoomButton()
    {
        currentPanel.SetActive(false);
        enterPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
