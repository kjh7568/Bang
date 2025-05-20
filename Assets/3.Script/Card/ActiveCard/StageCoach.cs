using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageCoach", menuName = "Card/Active/StageCoach")]
public class StageCoach : CardData
{
    public override void UseCard()
    {
        Debug.Log("역마차 사용");
    }
}
