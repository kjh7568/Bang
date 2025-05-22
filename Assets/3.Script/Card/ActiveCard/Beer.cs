using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Beer", menuName = "Card/Active/Beer")]
public class Beer : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("맥주 사용");
    }
}
