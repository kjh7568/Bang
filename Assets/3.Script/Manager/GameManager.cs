using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CardSystem cardSystem;
    
    private List<Player> players = new();
    
    private void Start()
    {
        CachePlayerInfo();
        SetPlayerHuman();
        SetPlayerJob();

        cardSystem.Init();
    }

    private void CachePlayerInfo()
    {
        foreach (var player in BasicSpawner.Instance.spawnedDummyPlayers)
        {
            var playerTemp = player.Value.GetComponent<Player>();
            players.Add(playerTemp);
            
            // Debug.Log($"PlayerRef: {player.Key}, NetworkObject: {playerTemp.BasicStat.nickName}");
        }
    }
    
    //인물과 직업은 카드가 아닌 시스템으로 인식하는 것으로 정했던 것 같아서 게임 매니저에서 만듬
    //구현의 포인트는 정해진 값이 서버에만 적용되는 것이 아닌 RPC를 통해 클라이언트에게도 정보를 넘겨줘야함
    //그 말은 즉, 해당 함수는 서버만 실행할 뿐 클라이언트는 이를 실행하면 안됨. --> IsServer를 활용 --> basicSpawner와 연동해야함
    private void SetPlayerHuman()
    {
        //일단 인물 구현이 완료 된 것이 아니기 때문에 그냥 더미로 만듬
        Debug.Log("각 플레이어의 인물 카드가 정해졌습니다.");
        
        /* foreach (var player in players)
           {
               player.GameStat.InGameStat.MyHuman =  player;
           } */
    }

    private void SetPlayerJob()
    {
        Debug.Log("각 플레이어의 직업이 정해졌습니다.");
    }
}
