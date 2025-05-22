using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jail", menuName = "Card/Passive/Jail")]
public class Jail : CardData
{
    public override bool RequiresTarget => true;
    
    public override void UseCard(Action onComplete)
    {
        Debug.Log("Jail : 플레이어 선택 중"); 

        UIManager.Instance.ShowPlayerSelectPanel((selectedPlayerName) => {
            Debug.Log($"선택된 플레이어: {selectedPlayerName}");
            Debug.Log("감옥 카드 효과 발동");

            // 다음 카드로 넘어가기
            onComplete?.Invoke(); 
        });
    }
}
