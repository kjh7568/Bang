using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WatingSetting : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionNumberText;
    [SerializeField] private GameObject startButton;
    
    private void Awake()
    {
        sessionNumberText.text = $"Session number: {BasicSpawner.Instance.GetSessionNumber()}";
    }

    public void ShowStartButton()
    {
        startButton.SetActive(true);
    }

    public void HideStartButton()
    {
        startButton.SetActive(false);
    }
}