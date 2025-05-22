using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WatingSetting : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionNumberText;
    [SerializeField] private TMP_Text[] nickNameTexts;
    [SerializeField] private Button startButton;

    public NetworkObject networkManager;
    
    private void Start()
    {
        sessionNumberText.text = $"Session number: {BasicSpawner.Instance.GetSessionNumber()}";
    }

    public void UpdateNicknameTexts(string[] nickNames)
    {
        for (int i = 0; i < nickNameTexts.Length; i++)
        {
            nickNameTexts[i].text = i < nickNames.Length ? nickNames[i] : "Empty";
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

    public async void OnStartButtonClicked()
    {
        var scene = SceneRef.FromIndex(3);
        if (!scene.IsValid) return;

        var runner = BasicSpawner.Instance._runner;

        Debug.Log("씬 로딩 시작");
        await runner.LoadScene(scene);
        Debug.Log("씬 로딩 완료됨");

        runner.Spawn(networkManager, Vector3.zero, Quaternion.identity);
        Debug.Log("네트워크 매니저 생성 완료");
    }

    public void OnBackButtonClicked()
    {
        BasicSpawner.Instance.LeaveSession();
    }
}