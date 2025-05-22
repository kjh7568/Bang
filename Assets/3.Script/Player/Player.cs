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
    public void RPC_ReceiveToHandCardsData(int[] handCardIds, RpcInfo info = default)
    {
        if (Runner.LocalPlayer != Object.InputAuthority) return;

        Debug.Log($"[Client] 카드 수신: {string.Join(",", handCardIds)}");

        var cards = new CardData[handCardIds.Length];
        
        for (int i = 0; i < handCardIds.Length; i++)
        {
            cards[i] = CardUIManager.Instance.GetCardByID(handCardIds[i]);
            
            if(cards[i]  == null) continue;
        }

        GameStat.InGameStat.HandCards = cards;
        
        CardUIManager.Instance.UpdateHandCardUI(cards);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartPlayerTurn(PlayerRef playerRef)
    {
        Debug.Log($"[Client] 플레이어 턴 시작: {playerRef}");

        if (Runner.LocalPlayer == playerRef)
        {
            // 내 턴이라면
            Debug.Log($"Runner.LocalPlayer :: {Runner.LocalPlayer}");
            UIManager.Instance.cardListPanel.SetActive(true);
            
            // 선택된 카드 인덱스를 서버에 보낼 버튼 이벤트 등록
            TurnManager.Instance.useCardButton.onClick.RemoveAllListeners();
            TurnManager.Instance.useCardButton.onClick.AddListener(() =>
            {
                int[] selectedIndices = UseCardUI.Instance.cardIndex.ToArray();
                Debug.Log($"[클라이언트] 선택된 카드 인덱스: {string.Join(",", selectedIndices)}");

                // RPC 호출
                this.RPC_RequestUseCardList(Runner.LocalPlayer, selectedIndices);

                // UI 정리
                UIManager.Instance.cardListPanel.SetActive(false);
            });

        }
        else
        {
            // 내 턴이 아니라면 
            UIManager.Instance.waitingPanel.SetActive(true);
            UIManager.Instance.waitingUserTurnText.text = "상대가 카드를 선택하는 중입니다.";
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUseCardList(PlayerRef playerRef, int[] cardIndices)
    {
        Debug.Log($"{playerRef} 클라이언트 → 카드 사용 요청");
        Debug.Log($"전달된 카드 인덱스 개수: {cardIndices.Length}");

        foreach (int index in cardIndices)
        {
            var card = GameStat.InGameStat.HandCards[index];
            card.UseCard(); 
        }
    }

}
