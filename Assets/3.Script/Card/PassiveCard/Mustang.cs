using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mustang", menuName = "Card/Passive/Mustang")]
public class Mustang : CardData
{
    public override void UseCard()
    {
        Debug.Log("야생마 장착");
    }
}
