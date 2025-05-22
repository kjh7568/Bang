using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayerSelectCardSetting : NetworkBehaviour
{
    public static PlayerSelectCardSetting Instance;
    
    private bool isLocalPlayer = false;
    private bool isPanelActive = false;

    public GameObject selectCardPanel;   
    public bool isPanelOn = false;
    
    public override void Spawned()
    {
        // Fusion 권한 검사
        isLocalPlayer = Object.HasInputAuthority;

        // 로컬 플레이어만 활성화
        enabled = isLocalPlayer;

        // 씬이 이미 InGame 상태라면—OnSceneLoaded는 이미 지나갔으니—여기서 직접 초기화
        if (isLocalPlayer && isPanelActive)
        {
            InitializePanel();
        }
    }

    private void Awake()
    {
        Instance = this;
        
        // 최초엔 꺼두고, Spawned()에서 켜집니다
        enabled = false;
        
        // 씬 전환 상태만 판단
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPanelActive = (scene.name == "4. InGame");

        // 호스트 쪽에서만 일어나서 권한이 세팅됐던 시점엔 Initialize 안 됐을 수 있으니
        if (isLocalPlayer && isPanelActive)
        {
            InitializePanel();
        }
    }

    public void InitializePanel()
    {
        selectCardPanel = UIPanelSynchronizer.Instance.selectCardPanel;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (selectCardPanel.activeInHierarchy)
            {
                selectCardPanel.SetActive(false);
                isPanelOn = false;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible    = false;
            }
            else
            {
                selectCardPanel.SetActive(true);
                isPanelOn = true;
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible    = true;
            }
        }
    }
}
