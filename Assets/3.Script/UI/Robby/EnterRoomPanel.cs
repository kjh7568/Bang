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
        SoundManager.Instance.PlaySound(SoundType.Button);

        Server.Instance.StartGame(GameMode.Client, roomNumberInput.text);
    }
    
    public void OnBackButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
