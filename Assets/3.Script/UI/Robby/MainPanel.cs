using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private GameObject currentPanel;
    [SerializeField] private GameObject selectRoomTypePanel;
    [SerializeField] private GameObject enterRoomPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject exitPanel;

    public void OnEnterRoomButton()
    {
        currentPanel.SetActive(false);
        selectRoomTypePanel.SetActive(true);
    }

    public void OnSettingButton()
    {
        currentPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        currentPanel.SetActive(false);
        exitPanel.SetActive(true);
    }
}