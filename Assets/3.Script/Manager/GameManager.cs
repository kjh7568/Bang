using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public HumanList humanList;
    public JobList jobList;

    // [SerializeField] private UINameSynchronizer uiSystem;
    //
    // [SerializeField] private VictoryCheck victoryCheck; // 승리 조건 체크용

    private Player turnOwner;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        if(!Server.Instance._runner.IsServer) return;
        
        // SetPlayerInfo();

        // SyncPlayersToClients();

        StartCoroutine(InitializeGame());
        
        
        
        CardSystem.Instance.Init();
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(2f);
        
        SetPlayerHuman();
        SetPlayerJob();
        
        turnOwner = GetFirstTurnPlayer();
        Broadcaster.Instance.RPC_StartPlayerTurn(turnOwner.playerRef);
    }
    
    // private void Update()
    // {
    //     // if (!BasicSpawner.Instance._runner.IsServer) return;
    //     //
    //     // victoryCheck.CheckVictoryConditions();
    //
    //     // while (true)
    //     // {
    //     //     RPC_StartPlayerTurn(turnOwner.playerRef);
    //     //
    //     //     bool taskRes = TaskCompletionSource<bool>();
    //     //
    //     //     if (taskRes)
    //     //     {
    //     //         currentTurnIndex++;
    //     //     }
    //     //
    //     //     RPC_StartPlayerTurn(Player.GetPlayer(currentTurnIndex).playerRef);
    //     // }
    // }

    private Player GetFirstTurnPlayer()
    {
        // for (int i = 0; i < Player.ConnectedPlayers.Count; i++)
        // {
        //     if (Player.LocalPlayer.playerGameStat.InGameStat.MyJob.Name == "보안관")
        //     {
        //         return Player.GetPlayer(i);
        //     }
        // }
        return Player.GetPlayer(Broadcaster.Instance.turnIdx);
        
        return null;
    }
    
    private void SetPlayerHuman()
    {
        var randomHumanList = Enumerable.Range(0, humanList.humanList.Count).OrderBy(_ => Random.value).ToList();
        
        for (int i = 1; i <= Player.ConnectedPlayers.Count; i++)
        {
            Broadcaster.Instance.RPC_SendPlayerHuman(Player.GetPlayer(i).playerRef, randomHumanList[i]);
        }
    }
    private void SetPlayerJob()
    {
        var randomJobList = Enumerable.Range(0, jobList.jobList.Count).OrderBy(_ => Random.value).ToList();
        
        for (int i = 1; i <= Player.ConnectedPlayers.Count; i++)
        {
            Broadcaster.Instance.RPC_SendPlayerJob(Player.GetPlayer(i).playerRef, randomJobList[i]);
        }
    }
    
    //
    // private void SetPlayerInfo() 
    // {
    //     foreach (var player in players)
    //     {
    //         cardSystem.Init();
    //         
    //         //todo 이거 수정
    //         uiSystem.Init(player);
    //     }
    // }
    //
    
    //
    //
    // public void SyncPlayersToClients()
    // {
    //     var playerRefsArray = playerRef.ToArray();  
    //     var playerClassArray = players.ToArray();  
    //
    //     // 초기 플레이어 정보 동기화
    //     Broadcaster.Instance.RPC_SyncSpawnedPlayers(playerRefsArray, playerClassArray);
    // }
    //
    // public void SetLocalPlayer(PlayerRef[] playerRefs)
    // {
    //     Debug.Log($"SetLocalPlayer 실행");
    //
    //     foreach (var playerRef in playerRefs)
    //     {
    //         Debug.Log($"SetLocalPlayer ::{Broadcaster.Instance.LocalRef}");
    //        
    //         if (Server.Instance.spawnedPlayers.TryGetValue(playerRef, out var obj))
    //         {
    //             var player = obj.GetComponent<Player>();
    //
    //             Broadcaster.Instance.LocalPlayer = player;
    //             Broadcaster.Instance.LocalRef = playerRef;
    //             // UIManager.Instance.localPlayer = playerRef;
    //
    //             // Debug.Log($"내 플레이어 설정 완료: {player.BasicStat.nickName}");
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"[SetLocalPlayer] spawnedPlayers에 {playerRef}가 없습니다.");
    //         }
    //
    //         break;
    //     }
    // }
}
