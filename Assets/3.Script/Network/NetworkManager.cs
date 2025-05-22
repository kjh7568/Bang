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
    
    // public static event Action OnNetworkManagerReady;
    //
    // public override void Spawned()
    // {
    //     base.Spawned();
    //     OnNetworkManagerReady?.Invoke();  
    // }

    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // public void SyncAllPlayersToClients()
    // {
    //     players = GameManager.Instance.players.ToArray();
    // }
    
    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // public void RPC_SyncPlayerInfo(PlayerRef[] playerRefs, string[] nicknames)
    // {
    //     for (int i = 0; i < playerRefs.Length; i++)
    //     {
    //         Debug.Log($"Player {i} - Ref: {playerRefs[i]}, Nickname: {nicknames[i]}");
    //         players[i] = playerRefs[i];
    //         // 클라이언트에서 각 플레이어 정보를 저장 또는 UI 업데이트
    //     }
    // }
  

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
}