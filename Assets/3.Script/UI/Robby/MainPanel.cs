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
        selectRoomTypePanel.SetActive(true);
    }

    public void OnSettingButton()
    {
        settingPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        exitPanel.SetActive(true);
    }
}