using System;
using CardTypeEnum;
using UnityEngine;

namespace CardTypeEnum
{
    public enum CardType
    {
        Active,
        Passive
    }
    
    public enum CardSymbol
    {
        Space,
        Diamond,
        Heart,
        Club
    }
}

public interface ICard
{
    int CardID { get; }
    string Name { get; }
    int Number { get; }
    CardType CardType { get; }
    CardSymbol CardSymbol { get; }
    Sprite CardSprite { get; }

    bool RequiresTarget { get; }
    
    public void UseCard(Action onComplete);
}