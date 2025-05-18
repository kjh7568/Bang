using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JesseJones", menuName = "Card/HumanData/JesseJones")]
public class JesseJones : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("카드 가져오기! 단계에서 첫 번째 카드를 다른 사람의 손에서 가져올 수도 있습니다.");
    }
}
