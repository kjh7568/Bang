using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectRoomTypePanel : MonoBehaviour
{
    [SerializeField] private GameObject previousPanel;
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private GameObject enterPanel;
    
    public void OnMakeRoomButton()
    {
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
