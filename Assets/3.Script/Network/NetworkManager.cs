using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkManager : NetworkBehaviour
{
    public static NetworkManager Instance;

    [Networked] public int TurnIndex {get; set;}
    public PlayerRef[] syncedPlayerRefs;
    public Player[] syncedPlayerClass;
    
    private void Awake()
    {
        Instance = this;
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
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetPanel()
    {
        UIManager.Instance.waitingPanel.SetActive(false);
        UIManager.Instance.cardListPanel.SetActive(false);
    }
}