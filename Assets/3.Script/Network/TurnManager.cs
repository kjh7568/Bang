using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    
    private int CurrentTurnIndex { get; set; }

    private List<PlayerRef> turnOrder = new List<PlayerRef>();
    
    private void Awake()
    {
        Instance = this;
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
        int random = Random.Range(0, GameManager.Instance.players.Count);

        CurrentTurnIndex = random;
        
        var player = GameManager.Instance.players[CurrentTurnIndex];
        player.RPC_StartPlayerTurn(turnOrder[CurrentTurnIndex]);
    }

    // public void ChangeTurn()
    // {
    //     
    // }

    public void EndTurn(PlayerRef fromPlayer)
    {
        // if (fromPlayer != ) return;
        //
        // CurrentTurnIndex = (CurrentTurnIndex + 1) % turnOrder.Count;
        // CurrentPlayerRef = turnOrder[CurrentTurnIndex];
        //
        // StartTurn();
    }
}