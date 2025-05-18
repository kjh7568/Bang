using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoseDoolan", menuName = "Card/HumanData/RoseDoolan")]
public class RoseDoolan : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("다른 사람을 볼 때 거리 1이 가까워집니다.");
    }
}