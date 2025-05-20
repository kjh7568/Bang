using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerGameStat playerGameStat;
    [SerializeField] private PlayerBasicStat playerBasicStat;
    
    public PlayerGameStat GameStat => playerGameStat;
    public PlayerBasicStat BasicStat => playerBasicStat;
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveHandCards(int[] cardIDs, RpcInfo info = default)
    {
        // 카드 할당
        var cards = new CardData[cardIDs.Length];
        for (int i = 0; i < cardIDs.Length; i++)
        {
            cards[i] = CardUIManager.Instance.GetCardByID(cardIDs[i]);
        }
        
        GameStat.InGameStat.HandCards = cards;
        PlayerRef myRef = Runner.LocalPlayer;
        Debug.Log($"myRef:: {myRef}");

        var playerObj = Runner.GetPlayerObject(myRef);
        var player = playerObj.GetComponent<Player>();
        Debug.Log(player.BasicStat.nickName);
        
        //RPC_RequestPlayerInfo();
        //RPC_RequestPlayerInfo_Fallback(Runner.LocalPlayer);
    }
    
        
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestPlayerInfo_Fallback(PlayerRef requester)
    {
        if (BasicSpawner.Instance.spawnedPlayers.TryGetValue(requester, out var playerObj))
        {
            var player = playerObj.GetComponent<Player>();
            string nickname = player.BasicStat.nickName;

            Debug.Log(nickname);
            //RPC_ReceivePlayerInfo(nickname, requester);
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPlayerInfo(RpcInfo info = default)
    {
        if (!Runner.IsServer) return; 

        // if (info.Source == null)
        // {
        //     Debug.LogWarning("info.Source is null. Check who is calling this RPC.");
        //     return;
        // }

        Debug.Log($"Runner Is Server :: {BasicSpawner.Instance.spawnedPlayers}");
        
        if (BasicSpawner.Instance.spawnedPlayers.TryGetValue(info.Source, out var playerObj))
        {
            var player = playerObj.GetComponent<Player>();
            string nickname = player.BasicStat.nickName;

            //RPC_ReceivePlayerInfo(nickname, info.Source);
        }
        else
        {
            Debug.LogWarning($"PlayerRef {info.Source} not found in spawnedPlayers.");
        }
    }
}
