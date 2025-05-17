using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField]private GameObject currentPanel;
    [SerializeField]private GameObject previousPanel;
    
    public void OnBackButton()
    {
        currentPanel.SetActive(false);
        previousPanel.SetActive(true);
    }
}
