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
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AttackPlayerNotify(Player local, Player target)
    {
        Debug.Log($"{local.BasicStat.nickName}님이 {target.BasicStat.nickName}을(를) 공격 대상으로 선택함");

        if (target == LocalPlayer)
        {
            UIManager.Instance.ShowCardSelectionPanel((selectedCardID) =>
            {                   
                // 카드 선택 완료 후
                RPC_TargetSelectedCard(local, target, selectedCardID);
            });
        }
        else if (local == LocalPlayer)
        {
            UIManager.Instance.ShowWaitingForTargetPanel();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TargetSelectedCard(Player attacker, Player target, int cardId)
    {
        var selectedCard = UIManager.Instance.localPlayer.GameStat.InGameStat.HandCards[cardId];
        Debug.Log($"{target.BasicStat.nickName}님이 {selectedCard.Name},{selectedCard.CardID} 카드를 선택함");

        TurnManager.Instance.ContinueTurn(attacker);
    }
}