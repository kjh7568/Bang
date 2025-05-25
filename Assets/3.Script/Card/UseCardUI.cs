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

        //대상 지정이 필요한 카드인지를 알고
        //필요하다면 패널을 킴
        
        Broadcaster.Instance.RPC_RequestUseCardList(BasicSpawner.Instance._runner.LocalPlayer, index);
    }
}
