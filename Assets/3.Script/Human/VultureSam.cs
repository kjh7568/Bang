using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VultureSam", menuName = "Card/HumanData/VultureSam")]
public class VultureSam : HumanData
{
    public override void UseAbility()
    {
        Debug.Log("게임에서 제거되는 사람이 생길 때마다, 그 사람의 모든 카드를 가져와 손에 듭니다.");
    }
}