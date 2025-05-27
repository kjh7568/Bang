using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleTest : NetworkBehaviour
{
    [SerializeField] private VictoryCheck victoryCheck; // 승리 조건 체크용
    [SerializeField] private TMP_Text NotifyText; // 전투 결과 텍스트 UI
        
    private void Update()
    {
        if (!HasStateAuthority) return; 
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyRandomDamage();
            string nick = Player.ConnectedPlayers[Random.Range(0, 4)].BasicStat.nickName;
            RPC_ShowNotifyText(nick);
            victoryCheck.CheckVictoryConditions();
        }
    }
   
   
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_ShowNotifyText(string nickname)
    {
        NotifyText.text = $"{nickname} 피해를 입었습니다 ";
        NotifyText.gameObject.SetActive(true);
    }
   
    private void ApplyRandomDamage()
    {
        List<Player> players = Player.ConnectedPlayers;
   
        if (players.Count == 0) return;
   
        // 살아있는 플레이어만 필터링
        List<Player> alivePlayers = players.FindAll(p => !p.InGameStat.IsDead);
   
        if (alivePlayers.Count == 0)
        {
            Debug.Log("모든 플레이어가 사망했습니다.");
            return;
        }
   
        // 무작위 플레이어 선택
        int randomIndex = Random.Range(0, alivePlayers.Count);
        Player selectedPlayer = alivePlayers[randomIndex];
   
        // 데미지 적용
        selectedPlayer.InGameStat.hp--;
   
        Debug.Log($"{selectedPlayer.BasicStat.nickName} 이(가) 데미지를 입었습니다. 남은 체력: {selectedPlayer.InGameStat.hp}");
   
        // 사망 판정
        if (selectedPlayer.InGameStat.hp <= 0)
        {
            selectedPlayer.InGameStat.hp = 0;
            Debug.Log($"{selectedPlayer.BasicStat.nickName} 이(가) 사망했습니다.");
        }
    }
}

