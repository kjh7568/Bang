using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatBalou", menuName = "Card/Active/CatBalou")]
public class CatBalou : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("캣벌로우 사용");
    }
}
