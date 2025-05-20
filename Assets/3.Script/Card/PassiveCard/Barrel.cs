using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barrel", menuName = "Card/Passive/Barrel")]
public class Barrel : CardData
{
    public override void UseCard()
    {
        Debug.Log("술통 장착");
    }
}
