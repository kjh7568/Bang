using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Schofield", menuName = "Card/Passive/Schofield")]
public class Schofield : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("스코필드 장착");
    }
}
