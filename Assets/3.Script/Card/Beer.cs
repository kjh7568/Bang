using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Beer", menuName = "Card/Beer")]
public class Beer : CardData
{
    public override void UseCard()
    {
        Debug.Log("맥주 사용");
    }
}
