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
        Debug.Log("Bang : 플레이어 선택 중"); 

        UIManager.Instance.ShowPlayerSelectPanel((selectedPlayerName) => {
            Debug.Log($"선택된 플레이어: {selectedPlayerName}");
            

            EffectBang();
            
            onComplete?.Invoke(); 
        });
    }


    public void EffectBang()
    {
        // 뱅 효과 처리
        Debug.Log("Bang 효과 적용 중");
    }
}
