using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Volcanic", menuName = "Card/Passive/Volcanic")]
public class Volcanic : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("볼캐닉 장착");
    }
}
