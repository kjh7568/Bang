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
        Debug.Log("Bang : 플레이어 선택 중"); 
        
        UIManager.Instance.ShowPlayerSelectPanel((selectedPlayerName) => {
            Debug.Log($"선택된 플레이어: {selectedPlayerName}");
            
            onComplete?.Invoke(); 
        });
    }


    public void EffectBang()
    {
        Debug.Log($"{BasicSpawner.Instance._runner.LocalPlayer}가 뱅을 사용!");
        Broadcaster.Instance.RPC_MakeCombatEvent(BasicSpawner.Instance._runner.LocalPlayer, BasicSpawner.Instance._runner.LocalPlayer, 1);
    }
}
