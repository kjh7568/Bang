using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;
    
    [Networked] public int TurnIndex {get; set;}
    public PlayerRef[] syncedPlayerRefs;
    public Player[] syncedPlayerClass;
    public Player LocalPlayer;
    
    private NetworkRunner networkRunner;
    
    private void Awake()
    {
        Instance = this;
        networkRunner = FindObjectOfType<NetworkRunner>();
        
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateNicknames(string[] nicknames)
    {
        WatingSetting ui = FindObjectOfType<WatingSetting>();
        if (ui != null)
            ui.UpdateNicknameTexts(nicknames);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendNicknameToHost(string nickname, RpcInfo info = default)
    {
        PlayerRef playerRef = info.Source;

        // 닉네임을 BasicSpawner에서 갱신
        if (BasicSpawner.Instance != null)
        {
            BasicSpawner.Instance.ReceiveNicknameFromClient(playerRef, nickname);
        }
        else
        {
            Debug.LogWarning("[Broadcaster] BasicSpawner 인스턴스를 찾을 수 없습니다.");
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncSpawnedPlayers(PlayerRef[] playerRefs, Player[] playerClass)
    {
        syncedPlayerRefs = new PlayerRef[playerRefs.Length];
        syncedPlayerClass = new Player[playerClass.Length];

        syncedPlayerClass = playerClass;
        syncedPlayerRefs = playerRefs;
        
        Debug.Log($"Received {playerRefs.Length} playerRefs");
        Debug.Log($"Received {playerClass.Length} playerClass");

        UIManager.Instance.SetTargetSelectionUI();
        GameManager.Instance.SetLocalPlayer(syncedPlayerRefs);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetPanel()
    {
        UIManager.Instance.waitingPanel.SetActive(false);
        UIManager.Instance.cardListPanel.SetActive(false);
    }
}