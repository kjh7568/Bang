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

    public void OnCardClicked(int index)
    {
        UIManager.Instance.cardListPanel.SetActive(false);
        
        var player = GameManager.Instance.players[TurnManager.Instance.CurrentTurnIndex];
        Debug.Log($"{player.BasicStat.nickName} 님이 선택한 카드:: {index}");
        player.RPC_RequestUseCardList(player.Runner.LocalPlayer, index);
    }
}
