using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Saloon", menuName = "Card/Active/Saloon")]
public class Saloon : CardData
{
    public override void UseCard()
    {
        Debug.Log("주점 사용");
    }
}
