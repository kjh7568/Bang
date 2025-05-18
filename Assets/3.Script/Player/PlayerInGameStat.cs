using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInGameStat
{
    public int hp;
    public int bulletRange;
    
    public ICard[] HandCards;
    
    public IHuman MyHuman;
    public IJob MyJob;

    public bool isBang;
    public bool isVolcanic;
    public bool isBarrel;
    public bool isMustang;
    public bool isJail;
    public bool isDynamite;
}
