using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dynamite", menuName = "Card/Passive/Dynamite")]
public class Dynamite : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("다이너마이트 사용");
    }
}
