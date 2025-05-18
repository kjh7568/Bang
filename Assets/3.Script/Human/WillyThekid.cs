using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WillyThekid", menuName = "Card/HumanData/WillyThekid")]
public class WillyThekid : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("<뱅!>을 원하는 만큼 사용할 수 있습니다.");
    }
}