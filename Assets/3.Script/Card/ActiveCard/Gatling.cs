using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gatling", menuName = "Card/Active/Gatling")]
public class Gatling : CardData
{
    public override void UseCard()
    {
        Debug.Log("기관총 사용");
    }
}
