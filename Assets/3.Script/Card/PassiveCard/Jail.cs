using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jail", menuName = "Card/Passive/Jail")]
public class Jail : CardData
{
    public override void UseCard()
    {
        Debug.Log("감옥 장착");
    }
}
