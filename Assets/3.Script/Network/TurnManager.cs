using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    
    //public int CurrentTurnIndex { get; set; }

    //public List<PlayerRef> turnOrder = new List<PlayerRef>();

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
            //turnOrder.Add(player.Object.InputAuthority);
        }
        
        foreach (var player in GameManager.Instance.players)
        {
            if (player.Runner.IsServer)
            {
                //NetworkManager.Instance.TurnIndex = CurrentTurnIndex;
            }
        }
        
        //StartTurn();
    }

    public void StartTurn()
    {
        Debug.Log($"playersRef: {NetworkManager.Instance.syncedPlayerRefs.Length}, TurnIndex: {NetworkManager.Instance.TurnIndex}");
        Debug.Log($"playerClass: {NetworkManager.Instance.syncedPlayerClass.Length}");

        for (int i = 0; i < NetworkManager.Instance.syncedPlayerClass.Length; i++)
        {
            Debug.Log($"playerClass{i}: {NetworkManager.Instance.syncedPlayerClass[i]}");
            
            if (NetworkManager.Instance.syncedPlayerClass[i].GameStat.InGameStat.MyJob.Name == "보안관")
            {
                Debug.Log($"{NetworkManager.Instance.syncedPlayerClass[i].BasicStat.nickName}님이 보안관 입니다.");
                NetworkManager.Instance.TurnIndex = i;
                
                break;
            }

            NetworkManager.Instance.TurnIndex = 0;
        }

        var currentPlayer = NetworkManager.Instance.syncedPlayerClass[NetworkManager.Instance.TurnIndex];
        currentPlayer.RPC_StartPlayerTurn(NetworkManager.Instance.syncedPlayerRefs[NetworkManager.Instance.TurnIndex]);
    }
    
    public void ChangeTurn()
    {
        Debug.Log($"턴 변경 전: {NetworkManager.Instance.TurnIndex}");
        
        var player = NetworkManager.Instance.syncedPlayerClass[NetworkManager.Instance.TurnIndex];

        Debug.Log($"턴 변경 후: {NetworkManager.Instance.TurnIndex}");

        player.RPC_RequestFinishTurn(player.Runner.LocalPlayer);
    }
    
    public PlayerRef EndTurn()
    {
        Debug.Log("EndTurn");
        
        NetworkManager.Instance.TurnIndex = (NetworkManager.Instance.TurnIndex + 1) % NetworkManager.Instance.syncedPlayerClass.Length;
        Debug.Log($"CurrentTurnIndex:: {NetworkManager.Instance.TurnIndex}");
        
        // var player = NetworkManager.Instance.syncedPlayerClass[ NetworkManager.Instance.TurnIndex];
        // player.RPC_TurnSync(turnOrder.ToArray(), NetworkManager.Instance.TurnIndex);
        
        return NetworkManager.Instance.syncedPlayerRefs[NetworkManager.Instance.TurnIndex];
    }
}