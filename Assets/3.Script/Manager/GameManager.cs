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

    // public List<Player> players;
    // public List<PlayerRef> playerRef;
    
    
    private void Awake()
    {
        Instance = this;
        
        
        Debug.Log($"나는 {Broadcaster.Instance.LocalPlayer}이다.");
    }

    private void Start()
    {
        if(!Server.Instance._runner.IsServer) return;
        
        // CachePlayerInfo();
        
        SetPlayerHuman();
        SetPlayerJob();
        
        GetPlayerInfo();
        
        TurnManager.Instance.StartTurn();
    }

    private void Update()
    {
        if (!Server.Instance._runner.IsServer) return;
        
        victoryCheck.CheckVictoryConditions();
    }

    // private void CachePlayerInfo()
    // {
    //     foreach (var player in BasicSpawner.Instance.spawnedPlayers.Values)
    //     {
    //         var playerClass = player.GetComponent<Player>();
    //
    //         players.Add(playerClass);
    //         playerRef.Add(player.InputAuthority);
    //     }
    // }
    
    private void SetPlayerHuman()
    {
        //일단 인물 구현이 완료 된 것이 아니기 때문에 그냥 더미로 만듬
        var tempList = new List<HumanData>(humanList.humanList);
        
        foreach (var player in Broadcaster.Instance.allPlayerClass)
        {
            int idx = Random.Range(0, tempList.Count);
            player.GameStat.InGameStat.MyHuman = tempList[idx];
            tempList.RemoveAt(idx);
        }
    }

    private void GetPlayerInfo() 
    {
        foreach (var player in Broadcaster.Instance.allPlayerClass)
        {
            var nickName = player.BasicStat.nickName;
            var human = player.GameStat.InGameStat.MyHuman.Name;
            var job = player.GameStat.InGameStat.MyJob.Name;
            
            Debug.Log($"{nickName} 플레이어의 인물은 {human}이며, 직업은 {job}입니다.");
            
            cardSystem.Init();
            uiSystem.Init(player);
        }
    }
    
    private void SetPlayerJob()
    {
        var tempList = new List<Job>(jobList.jobList);
        
        Broadcaster.Instance.allPlayerClass[0].GameStat.InGameStat.MyJob = tempList[0];
        Broadcaster.Instance.allPlayerClass[1].GameStat.InGameStat.MyJob = tempList[1];
        Broadcaster.Instance.allPlayerClass[2].GameStat.InGameStat.MyJob = tempList[2];
        Broadcaster.Instance.allPlayerClass[3].GameStat.InGameStat.MyJob = tempList[3];
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
    

    

    
    
    // public void SetLocalPlayer(PlayerRef[] playerRefs)
    // {
    //     foreach (var playerRef in playerRefs)
    //     {
    //         if (BasicSpawner.Instance.spawnedPlayers.TryGetValue(playerRef, out var obj))
    //         {
    //             var player = obj.GetComponent<Player>();
    //             Broadcaster.Instance.LocalPlayer = player;
    //             UIManager.Instance.localPlayer = playerRef;
    //             Broadcaster.Instance.LocalRef = playerRef;
    //             
    //             Debug.Log($"내 플레이어 설정 완료: {player.BasicStat.nickName}");
    //             
    //             break;
    //         }
    //     }
    // }
}
