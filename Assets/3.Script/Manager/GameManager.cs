using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public List<Player> players;
    public List<PlayerRef> playerRef;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CachePlayerInfo();
        
        SetPlayerHuman();
        SetPlayerJob();
        
        GetPlayerInfo();
        
        SyncPlayersToClients();
        TurnManager.Instance.StartTurn();
    }

    private void CachePlayerInfo()
    {
        foreach (var player in BasicSpawner.Instance.spawnedPlayers.Values)
        {
            var playerClass = player.GetComponent<Player>();

            players.Add(playerClass);
            playerRef.Add(player.InputAuthority);
        }
    }
    
    //인물과 직업은 카드가 아닌 시스템으로 인식하는 것으로 정했던 것 같아서 게임 매니저에서 만듬
    //구현의 포인트는 정해진 값이 서버에만 적용되는 것이 아닌 RPC를 통해 클라이언트에게도 정보를 넘겨줘야함
    //그 말은 즉, 해당 함수는 서버만 실행할 뿐 클라이언트는 이를 실행하면 안됨. --> IsServer를 활용 --> basicSpawner와 연동해야함
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

    private void GetPlayerInfo() 
    {
        foreach (var player in players)
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
        
        foreach (var player in players)
        {
            int idx = Random.Range(0, tempList.Count);
            player.GameStat.InGameStat.MyJob = tempList[idx];
            tempList.RemoveAt(idx);
        }
    }
    

    public void SyncPlayersToClients()
    {
        var playerRefsArray = playerRef.ToArray();  
        var playerClassArray = players.ToArray();  

        // 초기 플레이어 정보 동기화
        Broadcaster.Instance.RPC_SyncSpawnedPlayers(playerRefsArray, playerClassArray);
    }

    public void SetLocalPlayer(PlayerRef[] playerRefs)
    {
        foreach (var playerRef in playerRefs)
        {
            if (BasicSpawner.Instance.spawnedPlayers.TryGetValue(playerRef, out var obj))
            {
                var player = obj.GetComponent<Player>();
                Broadcaster.Instance.LocalPlayer = player;
                UIManager.Instance.localPlayer = playerRef;
                Broadcaster.Instance.LocalRef = playerRef;
                
                Debug.Log($"내 플레이어 설정 완료: {player.BasicStat.nickName}");
                
                break;
            }
        }
    }
    
    public Player GetPlayer(PlayerRef playerRef)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Object.InputAuthority == playerRef)
                return players[i];
        }

        return null;
    }
}
