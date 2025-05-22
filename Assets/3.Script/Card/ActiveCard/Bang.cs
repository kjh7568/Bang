using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "Bang", menuName = "Card/Active/Bang")]
public class Bang : CardData
{
    //public override bool RequiresTarget => true;

    public override void UseCard(Action onComplete)
    {
        Debug.Log("Bang : 플레이어 선택 중"); 

        UIManager.Instance.ShowPlayerSelectPanel((selectedPlayerName) => {
            Debug.Log($"선택된 플레이어: {selectedPlayerName}");
            Debug.Log("뱅 카드 효과 발동");
            
            // 지훈이한테 쓸거야
            // network -> 지훈이한테 data를 받아 -> 빗나감 카드가 있으면 방어 아니면 공격
            // network server
            
            // 다음 카드로 넘어가기
            onComplete?.Invoke(); 
        });

        // if (RequiresTarget)
        // {
        //     Debug.Log("플레이어 선택 시작");
        //
        //     UIManager.Instance.ShowPlayerSelectPanel((string selectedPlayerName) => {
        //         Debug.Log($"선택된 플레이어: {selectedPlayerName}");
        //
        //         Debug.Log("뱅 카드 효과 발동");
        //
        //         // 다음 카드로 넘어가기
        //         onComplete?.Invoke(); 
        //     });
        // }
        // else
        // {
        //     Debug.Log("뱅 카드: 타겟 없이 발동");
        //     onComplete?.Invoke();
        // }
    }
}
