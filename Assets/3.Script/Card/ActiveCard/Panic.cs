using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Panic", menuName = "Card/Active/Panic")]
public class Panic : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("강탈 사용");
    }
}
