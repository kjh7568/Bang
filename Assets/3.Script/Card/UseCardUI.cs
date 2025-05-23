using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCardUI : MonoBehaviour
{
    public static UseCardUI Instance;
    //public List<int> cardIndex = new List<int>();
    
    private void Awake()
    {
        Instance = this;
    }

    // public void OnCardClicked(int index)
    // {
    //     UIManager.Instance.cardListPanel.SetActive(false);
    //     
    //     var player = GameManager.Instance.players[Broadcaster.Instance.TurnIndex];
    //     Debug.Log($"{player.BasicStat.nickName} 님이 선택한 카드:: {index}");
    //     player.RPC_RequestUseCardList(player.Runner.LocalPlayer, index);
    //     UIManager.Instance.OnCardSelected(index);
    // }
    
    public void OnCardClicked(int index)
    {
        UIManager.Instance.cardListPanel.SetActive(false);

        Debug.Log($"Player:: {Broadcaster.Instance.LocalPlayer}");

        
        Debug.Log($"Broadcaster.Instance.TurnIndex:: {Broadcaster.Instance.TurnIndex}");
        Debug.Log($"Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex]:: { Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex]}");

        var player = Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex];
        Debug.Log(player);
        var selectedCard = player.GameStat.InGameStat.HandCards[index];
        Debug.Log(selectedCard);

        //Debug.Log($"{player.BasicStat.nickName} 님이 선택한 카드: {selectedCard.Name}");

        player.RPC_RequestUseCardList(player.Runner.LocalPlayer, index);

        UIManager.Instance.OnCardSelected(index);
    }
}
