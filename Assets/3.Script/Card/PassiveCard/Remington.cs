using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Remington", menuName = "Card/Passive/Remington")]
public class Remington : CardData
{
    public override void UseCard()
    {
        Debug.Log("레밍턴 장착");
    }
}
