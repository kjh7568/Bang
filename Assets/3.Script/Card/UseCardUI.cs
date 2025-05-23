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

        Debug.Log($"Player:: {BasicSpawner.Instance._runner.LocalPlayer}");
        Debug.Log($"Broadcaster.Instance.TurnIndex:: {Broadcaster.Instance.TurnIndex}");
        Debug.Log($"Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex]:: { Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex]}");

        
        var player = GameManager.Instance.GetPlayer(BasicSpawner.Instance._runner.LocalPlayer);
        
        var selectedCard = player.GameStat.InGameStat.HandCards[index];
        Debug.Log($"{player.BasicStat.nickName} 님이 {selectedCard.Name} / {selectedCard.CardID} 카드를 사용하였습니다.");


        player.RPC_RequestUseCardList(player.Runner.LocalPlayer, index);

        UIManager.Instance.OnCardSelected(index);
    }
}
