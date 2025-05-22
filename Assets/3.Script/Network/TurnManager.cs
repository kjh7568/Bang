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
        // int random = Random.Range(0, GameManager.Instance.players.Count);

        int random = 0;
        CurrentTurnIndex = random;
        
        var player = GameManager.Instance.players[CurrentTurnIndex];
        player.RPC_StartPlayerTurn(turnOrder[CurrentTurnIndex]);
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
        //if (fromPlayer != turnOrder[CurrentTurnIndex]) return;
        
        CurrentTurnIndex = (CurrentTurnIndex + 1) % turnOrder.Count;
        return turnOrder[CurrentTurnIndex + 1];
        
        //StartTurn();
    }
}