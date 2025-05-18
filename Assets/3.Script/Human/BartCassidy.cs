using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BartCassidy", menuName = "Card/Human/BartCassidy")]
public class BartCassidy : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("생명력 1을 잃을 때마다 카드 더미에서 카드 한 장을 가져옵니다.");
    }
}
