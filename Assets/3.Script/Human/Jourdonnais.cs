using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jourdonnais", menuName = "Card/HumanData/Jourdonnais")]
public class Jourdonnais : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("<뱅!>의 표적이 될 때마다 \"카드 펼치기!\"를 할 수 있으며, 하트가 나오면 총알이 빗나갑니다.");
    }
}