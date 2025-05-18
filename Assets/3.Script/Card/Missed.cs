using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Missed", menuName = "Card/Active/Missed")]
public class Missed : CardData
{
    public override void UseCard()
    {
        Debug.Log("빗나감 사용");
    }
}
