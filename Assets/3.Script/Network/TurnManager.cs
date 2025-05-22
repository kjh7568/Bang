using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    
    public int CurrentTurnIndex { get; set; }

    private List<PlayerRef> turnOrder = new List<PlayerRef>();

    public Button finishButton;

    private void Awake()
    {
        Instance = this;
            
        finishButton.onClick.AddListener(ChangeTurn);
    }
    
    public void InitializeTurnOrder()
    {
        foreach (var player in GameManager.Instance.players)
        {
            turnOrder.Add(player.Object.InputAuthority);
        }

        StartTurn();
    }

    public void StartTurn()
    {
        for (int i = 0; i < GameManager.Instance.players.Count; i++)
        {
            var player = GameManager.Instance.players[i];

            if (player.GameStat.InGameStat.MyJob.Name == "보안관")
            {
                Debug.Log($"{player.BasicStat.nickName}님이 보안관 입니다.");
                CurrentTurnIndex = i;
                
                break;
            }
        }

        var currentPlayer = GameManager.Instance.players[CurrentTurnIndex];
        
        currentPlayer.RPC_StartPlayerTurn(turnOrder[CurrentTurnIndex]);
    }


    public void ChangeTurn()
    {
        var player = GameManager.Instance.players[CurrentTurnIndex];
        
        player.RPC_RequestFinishTurn(player.Runner.LocalPlayer, CurrentTurnIndex);
    }
    
    public bool IsMyTurn()
    {
        var player = GameManager.Instance.players[CurrentTurnIndex];
        return turnOrder[CurrentTurnIndex] == player.Runner.LocalPlayer;
    }

    public PlayerRef EndTurn()
    {
        Debug.Log("EndTurn");
        
        CurrentTurnIndex = (CurrentTurnIndex + 1) % turnOrder.Count;
        Debug.Log($"CurrentTurnIndex:: {CurrentTurnIndex}");

        return turnOrder[CurrentTurnIndex];
    }
}