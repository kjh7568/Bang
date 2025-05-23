using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInGameStat : IDamageAble
{
    public int hp;
    public bool IsDead => hp <= 0;
    public int bulletRange;
    
    public ICard[] HandCards = new CardData[5];
    public int[] HandCardsId = new int[5];
    
    public IHuman MyHuman;
    public IJob MyJob;

    public bool isBang;
    public bool isVolcanic;
    public bool isBarrel;
    public bool isMustang;
    public bool isJail;
    public bool isDynamite;
    //

    public void TakeDamage(CombatEvent combatEvent)
    {
        hp -= combatEvent.Damage;

        if (hp <= 0)
        {
            Debug.Log("죽었슈");
        }
    }
}
