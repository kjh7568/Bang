using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralStore", menuName = "Card/Active/GeneralStore")]
public class GeneralStore : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("잡화점 사용");
    }
}
