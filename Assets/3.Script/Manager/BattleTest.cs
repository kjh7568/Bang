using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleTest : MonoBehaviour
{
    [SerializeField] private VictoryCheck victoryCheck; // 승리 조건 체크용
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyRandomDamage();
            victoryCheck.CheckVictoryConditions();
        }
    }

    private void ApplyRandomDamage()
    {
        List<Player> players = GameManager.Instance.players;

        if (players.Count == 0) return;

        // 살아있는 플레이어만 필터링
        List<Player> alivePlayers = players.FindAll(p => !p.GameStat.InGameStat.IsDead);

        if (alivePlayers.Count == 0)
        {
            Debug.Log("모든 플레이어가 사망했습니다.");
            return;
        }

        // 무작위 플레이어 선택
        int randomIndex = Random.Range(0, alivePlayers.Count);
        Player selectedPlayer = alivePlayers[randomIndex];

        // 데미지 적용
        selectedPlayer.GameStat.InGameStat.hp--;

        Debug.Log($"{selectedPlayer.BasicStat.nickName} 이(가) 데미지를 입었습니다. 남은 체력: {selectedPlayer.GameStat.InGameStat.hp}");

        // 사망 판정
        if (selectedPlayer.GameStat.InGameStat.hp <= 0)
        {
            selectedPlayer.GameStat.InGameStat.hp = 0;
            Debug.Log($"{selectedPlayer.BasicStat.nickName} 이(가) 사망했습니다.");
        }
    }
}

