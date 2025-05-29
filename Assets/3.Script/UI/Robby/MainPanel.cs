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
        SoundManager.Instance.PlaySound(SoundType.Button);
        currentPanel.SetActive(false);
        selectRoomTypePanel.SetActive(true);
    }

    public void OnSettingButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);
        currentPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);
        currentPanel.SetActive(false);
        exitPanel.SetActive(true);
    }
}