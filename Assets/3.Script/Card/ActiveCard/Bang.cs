using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "Bang", menuName = "Card/Active/Bang")]
public class Bang : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("뱅 실행!");
        EffectBang();
        
        // Debug.Log("Bang : 플레이어 선택 중"); 
        //
        // UIManager.Instance.ShowPlayerSelectPanel((selectedPlayerName) => {
        //     Debug.Log($"선택된 플레이어: {selectedPlayerName}");
        //     
        //
        //     
        //     
        //     onComplete?.Invoke(); 
        // });
    }


    public void EffectBang()
    {
        // 뱅 효과 처리// 기능 구현
        var senderRef = BasicSpawner.Instance._runner.LocalPlayer;
        var target = InGameSystem.Instance.GetPlayerOrNull(senderRef/*지목 대상 플레이어의 playerRef를 받아와야함. 즉, 다시 바꿀 것*/);

        if (target != null)
        {
            CombatEvent combatEvent = new CombatEvent
            {
                Sender = BasicSpawner.Instance.spawnedPlayers[senderRef].GetComponent<Player>().GameStat.InGameStat,
                Receiver = target,
                Damage = 1
            };

            InGameSystem.Instance.AddInGameEvent(combatEvent);
        }
    }
}
