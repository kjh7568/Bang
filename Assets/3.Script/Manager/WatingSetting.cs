using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WatingSetting : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionNumberText;
    [SerializeField] private TMP_Text[] nickNameTexts;
    
    [SerializeField] private Button startButton;
    
    private void Start()
    {
        sessionNumberText.text = $"Session number: {BasicSpawner.Instance.GetSessionNumber()}";
    }

    public void UpdateNicknameTexts(List<string> nicknames)
    {
        for (int i = 0; i < nickNameTexts.Length; i++)
        {
            if (i < nicknames.Count)
            {
                nickNameTexts[i].text = nicknames[i];
            }
            else
            {
                nickNameTexts[i].text = "Empty";
            }
        }
    }
    
    public void ShowStartButton()
    {
        startButton.gameObject.SetActive(true);
    }

    public void HideStartButton()
    {
        startButton.gameObject.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        var scene = SceneRef.FromIndex(3); 
        if (scene.IsValid) BasicSpawner.Instance._runner.LoadScene(scene);
    }
}