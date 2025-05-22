using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardTypeEnum;

public class CardData : ScriptableObject, ICard
{
    [SerializeField] private int cardID;
    [SerializeField] private string name;
    [SerializeField] private int number;
    [SerializeField] private CardType cardType;
    [SerializeField] private CardSymbol cardSymbol;
    [SerializeField] private Sprite cardSprite;

    public int CardID => cardID;
    public string Name => name;
    public int Number => number;
    public CardType CardType => cardType;
    public CardSymbol CardSymbol => cardSymbol;
    public Sprite CardSprite => cardSprite;

    public virtual bool RequiresTarget => false;
    
    public virtual void UseCard(Action onComplete)
    {
        // 기능 구현
        var target = InGameSystem.Instance.GetPlayerOrNull(/*지목 대상 플레이어의 playerRef를 받아와야함*/);

        if (target != null)
        {
            CombatEvent combatEvent = new CombatEvent
            {
                Sender = /*자신의 playerRef*/,
                Receiver = target,
                Damage = 1
            };

            InGameSystem.Instance.AddInGameEvent(combatEvent);
        }
    }
}
