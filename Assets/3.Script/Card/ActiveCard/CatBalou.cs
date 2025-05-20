using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatBalou", menuName = "Card/Active/CatBalou")]
public class CatBalou : CardData
{
    public override void UseCard()
    {
        Debug.Log("캣벌로우 사용");
    }
}
