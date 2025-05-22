using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Duel", menuName = "Card/Active/Duel")]
public class Duel : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("결투 사용");
    }
}
