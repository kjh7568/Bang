using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Indians", menuName = "Card/Active/Indians")]
public class Indians : CardData
{
    public override void UseCard()
    {
        Debug.Log("인디언 사용");
    }
}
