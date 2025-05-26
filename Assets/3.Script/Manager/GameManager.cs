using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private CardSystem cardSystem;
    [SerializeField] private UINameSynchronizer uiSystem;
    
    [SerializeField] private HumanList humanList;
    [SerializeField] private JobList jobList;
    [SerializeField] private VictoryCheck victoryCheck; // 승리 조건 체크용

    public List<Player> players;
    public List<PlayerRef> playerRef;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(!Server.Instance._runner.IsServer) return;
        
        // CachePlayerInfo();
        //
        // SetPlayerHuman();
        // SetPlayerJob();
        //
        // SetPlayerInfo();
        //
        // SyncPlayersToClients();
        
        StartGame();
    }

    // private void Update()
    // {
    //     if (!BasicSpawner.Instance._runner.IsServer) return;
    //     
    //     victoryCheck.CheckVictoryConditions();
    // }

    public void StartGame()
    {
        for (int i = 0; i < Broadcaster.Instance.syncedPlayerClass.Length; i++)
        {
            if (Broadcaster.Instance.syncedPlayerClass[i].GameStat.InGameStat.MyJob.Name == "보안관")
            {
                Broadcaster.Instance.TurnIndex = i;
                
                break;
            }
        
            // Broadcaster.Instance.TurnIndex = 0;
        }
        
        var currentPlayer = Broadcaster.Instance.syncedPlayerClass[Broadcaster.Instance.TurnIndex];
        
        Broadcaster.Instance.RPC_StartPlayerTurn(Broadcaster.Instance.syncedPlayerRefs[Broadcaster.Instance.TurnIndex]);
    }
    
    private void CachePlayerInfo()
    {
        foreach (var player in Server.Instance.spawnedPlayers.Values)
        {
            var playerClass = player.GetComponent<Player>();

            players.Add(playerClass);
            playerRef.Add(player.InputAuthority);
        }
    }
    
    private void SetPlayerHuman()
    {
        //일단 인물 구현이 완료 된 것이 아니기 때문에 그냥 더미로 만듬
        var tempList = new List<HumanData>(humanList.humanList);
        
        foreach (var player in players)
        {
            int idx = Random.Range(0, tempList.Count);
            player.GameStat.InGameStat.MyHuman = tempList[idx];
            tempList.RemoveAt(idx);
        }
    }

    private void SetPlayerInfo() 
    {
        foreach (var player in players)
        {
            cardSystem.Init();
            
            //todo 이거 수정
            uiSystem.Init(player);
        }
    }
    
    private void SetPlayerJob()
    {
        var tempList = new List<Job>(jobList.jobList);
        
        players[0].GameStat.InGameStat.MyJob = tempList[0];
        players[1].GameStat.InGameStat.MyJob = tempList[1];
        // players[2].GameStat.InGameStat.MyJob = tempList[2];
        // players[3].GameStat.InGameStat.MyJob = tempList[3];
    }
    // private void SetPlayerJob()
    // {
    //     var tempList = new List<Job>(jobList.jobList);
    //     
    //     foreach (var player in players)
    //     {
    //         int idx = Random.Range(0, tempList.Count);
    //         player.GameStat.InGameStat.MyJob = tempList[idx];
    //         tempList.RemoveAt(idx);
    //     }
    // }
    

    public void SyncPlayersToClients()
    {
        var playerRefsArray = playerRef.ToArray();  
        var playerClassArray = players.ToArray();  

        // 초기 플레이어 정보 동기화
        Broadcaster.Instance.RPC_SyncSpawnedPlayers(playerRefsArray, playerClassArray);
    }

    public void SetLocalPlayer(PlayerRef[] playerRefs)
    {
        Debug.Log($"SetLocalPlayer 실행");

        foreach (var playerRef in playerRefs)
        {
            Debug.Log($"SetLocalPlayer ::{Broadcaster.Instance.LocalRef}");
           
            if (Server.Instance.spawnedPlayers.TryGetValue(playerRef, out var obj))
            {
                var player = obj.GetComponent<Player>();

                Broadcaster.Instance.LocalPlayer = player;
                Broadcaster.Instance.LocalRef = playerRef;
                // UIManager.Instance.localPlayer = playerRef;

                // Debug.Log($"내 플레이어 설정 완료: {player.BasicStat.nickName}");
            }
            else
            {
                Debug.LogWarning($"[SetLocalPlayer] spawnedPlayers에 {playerRef}가 없습니다.");
            }

            break;
        }
    }
}
