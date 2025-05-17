using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class EnterRoomPanel : MonoBehaviour
{
    [SerializeField] private GameObject previousPanel;
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private TMP_InputField roomNumberInput;
    
    public void OnJoinButton()
    {
        BasicSpawner.Instance.StartGame(GameMode.Client, roomNumberInput.text);
    }
    
    public void OnBackButton()
    {
        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
