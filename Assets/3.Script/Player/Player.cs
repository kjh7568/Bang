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
    public void RPC_ReceiveHandCards(int[] cardIDs,RpcInfo info = default)
    { 
        var cards = new CardData[cardIDs.Length];
        
        // 카드 할당
        if (Runner.IsServer)
        {
            for (int i = 0; i < cardIDs.Length; i++)
            {
                cards[i] = CardUIManager.Instance.GetCardByID(cardIDs[i]);
            }
            
            GameStat.InGameStat.HandCards = cards;
        }

        RPC_ReceiveToHandCardsData();
    }
    
     
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveToHandCardsData(RpcInfo info = default)
    {
        PlayerRef myRef = Runner.LocalPlayer;
        Debug.Log($"myRef:: {myRef}");
        var playerObj = Runner.GetPlayerObject(myRef);
        var player = playerObj.GetComponent<Player>();
        
        CardUIManager.Instance.UpdateHandCardUI(player.GameStat.InGameStat.HandCards);
    }
}
