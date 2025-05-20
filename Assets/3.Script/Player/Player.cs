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

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // public void RPC_ReceiveHandCards(int[] cardIDs, RpcInfo info = default)
    // {
    //     if (!Object.HasInputAuthority) return;
    //
    //     var cards = new CardData[cardIDs.Length];
    //     
    //     for (int i = 0; i < cardIDs.Length; i++)
    //     {
    //         cards[i] = CardUIManager.Instance.GetCardByID(cardIDs[i]);
    //     }
    //
    //     GameStat.InGameStat.HandCards = cards;
    // }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveHandCards(int[] cardIDs, RpcInfo info = default)
    {
        //if (!Object.HasInputAuthority) return;

        // 카드 할당
        var cards = new CardData[cardIDs.Length];
        for (int i = 0; i < cardIDs.Length; i++)
        {
            cards[i] = CardUIManager.Instance.GetCardByID(cardIDs[i]);
        }
        
        GameStat.InGameStat.HandCards = cards;
        PlayerRef myRef = Runner.LocalPlayer;
        Debug.Log($"myRef:: {myRef}");

        RPC_RequestPlayerInfo_Fallback(Runner.LocalPlayer);
        
        // if (BasicSpawner.Instance.spawnedPlayers.TryGetValue(Runner.LocalPlayer, out var playerObj))
        // {
        //     var player = playerObj.GetComponent<Player>();
        //     Debug.Log($"player:: {player.BasicStat.nickName}");
        //
        //     CardUIManager.Instance.UpdateHandCardUI(cards);
        // }
        
        //StartCoroutine(WaitForPlayerObject(cards));

        //Debug.Log($"playerNetworkObject: {playerNetworkObject}");
        
        // Player 컴포넌트 가져오기 (NetworkRunner 필요)
        //NetworkObject playerNetworkObject = Runner.GetPlayerObject(myRef);
        //Debug.Log($"playerNetworkObject:: {playerNetworkObject}");
        
        // if (playerNetworkObject != null)
        // {
        //     Player playerComponent = playerNetworkObject.GetComponent<Player>();
        //     Debug.Log($"playerComponent:: {playerComponent}");
        //
        //     if (playerComponent != null)
        //     {
        //         Debug.Log($"[RPC] Player Component Found: {playerComponent.name}");
        //         // playerComponent를 통해 원하는 작업 수행
        //
        //         CardUIManager.Instance.UpdateHandCardUI(cards);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("[RPC] Player 컴포넌트 없음");
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("[RPC] Player NetworkObject 못 찾음");
        // }
    }

    
    IEnumerator WaitForPlayerObject(CardData[] cards)
    {
        yield return new WaitUntil(() => Runner.GetPlayerObject(Runner.LocalPlayer) != null);

        var playerNetworkObject = Runner.GetPlayerObject(Runner.LocalPlayer);
        Debug.Log($"playerNetworkObject: {playerNetworkObject}");

        var playerComponent = playerNetworkObject.GetComponent<Player>();
        if (playerComponent != null)
        {
            Debug.Log($"Player 컴포넌트 찾음: {playerComponent.name}");
            CardUIManager.Instance.UpdateHandCardUI(cards);
        }
        else
        {
            Debug.LogWarning("Player 컴포넌트 못 찾음");
        }
    }
}
