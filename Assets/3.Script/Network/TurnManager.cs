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

    public Button useCardButton;

    private void Awake()
    {
        Instance = this;
            
        useCardButton.onClick.AddListener(ChangeTurn);
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

    public void ChangeTurn()
    {
        var player = GameManager.Instance.players[CurrentTurnIndex];
        int[] selectedIndices = UseCardUI.Instance.cardIndex.ToArray();
        player.RPC_RequestUseCardList(player.Runner.LocalPlayer, selectedIndices);
        
        //player.RPC_RequestUseCardList(turnOrder[CurrentTurnIndex]);
    }
    
    public bool IsMyTurn()
    {
        var player = GameManager.Instance.players[CurrentTurnIndex];
        return turnOrder[CurrentTurnIndex] == player.Runner.LocalPlayer;
    }

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