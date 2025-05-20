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
    public void RPC_ReceiveHandCards(RpcInfo info = default)
    {
        PlayerRef myRef = Runner.LocalPlayer;
        Debug.Log($"myRef:: {myRef}");
        var playerObj = Runner.GetPlayerObject(myRef);
        var player = playerObj.GetComponent<Player>();
        var cards = new CardData[player.GameStat.InGameStat.HandCards.Length];
        
        Debug.Log(player.BasicStat.nickName);
        
        // 카드 할당
        if (Runner.IsServer)
        {
            for (int i = 0; i < player.GameStat.InGameStat.HandCards.Length; i++)
            {
                cards[i] = CardUIManager.Instance.GetCardByID(player.GameStat.InGameStat.HandCardsId[i]);
            }
        
            GameStat.InGameStat.HandCards = cards;
        }
        
        CardUIManager.Instance.UpdateHandCardUI(cards);
    }
}
