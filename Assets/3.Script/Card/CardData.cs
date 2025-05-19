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

    public virtual void UseCard()
    {
        // 기능 구현
    }
}
