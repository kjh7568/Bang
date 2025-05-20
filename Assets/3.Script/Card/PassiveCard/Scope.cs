using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scope", menuName = "Card/Passive/Scope")]
public class Scope : CardData
{
    public override void UseCard()
    {
        Debug.Log("스코프 장착");
    }
}
