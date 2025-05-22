using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WellsFargo", menuName = "Card/Active/WellsFargo")]
public class WellsFargo : CardData
{
    public override void UseCard(Action onComplete)
    {
        Debug.Log("웰스파고 은행 사용");
    }
}
