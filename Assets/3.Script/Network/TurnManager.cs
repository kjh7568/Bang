using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public Button finishButton;

    private void Awake()
    {
        Instance = this;
            
        finishButton.onClick.AddListener(ChangeTurn);
    }

    public void StartTurn()
    {
        Debug.Log($"playersRef: {Broadcaster.Instance.syncedPlayerRefs.Length}, TurnIndex: {Broadcaster.Instance.TurnIndex}");
        Debug.Log($"playerClass: {Broadcaster.Instance.syncedPlayerClass.Length}");

        for (int i = 0; i < Broadcaster.Instance.syncedPlayerClass.Length; i++)
        {
            Debug.Log($"playerClass{i}: {Broadcaster.Instance.syncedPlayerClass[i]}");
            
            if (Broadcaster.Instance.syncedPlayerClass[i].GameStat.InGameStat.MyJob.Name == "보안관")
            {
                Debug.Log($"{Broadcaster.Instance.syncedPlayerClass[i].BasicStat.nickName}님이 보안관 입니다.");
                Broadcaster.Instance.TurnIndex = i;
                
                break;
            }

            Broadcaster.Instance.TurnIndex = 0;
        }

        var currentPlayer = Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex];
        currentPlayer.RPC_StartPlayerTurn(Broadcaster.Instance.syncedPlayerRefs[Broadcaster.Instance.TurnIndex]);
    }
    
    public void ChangeTurn()
    {
        Debug.Log($"턴 변경 전: {Broadcaster.Instance.TurnIndex}");
        
        var player = Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex];

        Debug.Log($"턴 변경 후: {Broadcaster.Instance.TurnIndex}");

        player.RPC_RequestFinishTurn(player.Runner.LocalPlayer);
    }
    
    public PlayerRef EndTurn()
    {
        Debug.Log("EndTurn");
        
        Broadcaster.Instance.TurnIndex = (Broadcaster.Instance.TurnIndex + 1) % Broadcaster.Instance.syncedPlayerClass.Length;
        Debug.Log($"CurrentTurnIndex:: {Broadcaster.Instance.TurnIndex}");
   
        return Broadcaster.Instance.syncedPlayerRefs[Broadcaster.Instance.TurnIndex];
    }
    
    public void ContinueTurn(Player player)
    {
        Debug.Log($"{player.BasicStat.nickName}의 턴 계속됨");
        
        //UIManager.Instance.OnCardSelected();
    }
}