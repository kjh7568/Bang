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

    
    
    public void ChangeTurn()
    {
        Debug.Log($"턴 변경 전: {Broadcaster.Instance.TurnIndex}");
        
        var player = Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex];

        Debug.Log($"턴 변경 후: {Broadcaster.Instance.TurnIndex}");

        Broadcaster.Instance.RPC_RequestFinishTurn(player.Runner.LocalPlayer);
    }
    
    public PlayerRef EndTurn()
    {
        Debug.Log("EndTurn");
        
        Broadcaster.Instance.TurnIndex = (Broadcaster.Instance.TurnIndex + 1) % Broadcaster.Instance.syncedPlayerClass.Length;
        Debug.Log($"CurrentTurnIndex:: {Broadcaster.Instance.TurnIndex}");
   
        return Broadcaster.Instance.syncedPlayerRefs[Broadcaster.Instance.TurnIndex];
    }
    
    public void ContinueTurn(PlayerRef playerRef)
    {
        var player = Broadcaster.Instance.GetPlayer(playerRef);
        
        Debug.Log($"{player.BasicStat.nickName}의 턴 계속됨");
        
        UIManager.Instance.ShowCardTargetPanel();
    }
}