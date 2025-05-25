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
    [SerializeField] private bool isTargetRequired;
    private ICard _cardImplementation;

    public int CardID => cardID;
    public string Name => name;
    public int Number => number;
    public CardType CardType => cardType;
    public CardSymbol CardSymbol => cardSymbol;
    public Sprite CardSprite => cardSprite;
    public bool IsTargetRequired => isTargetRequired;

    public virtual void UseCard() { }
    
    public virtual void UseCard(Action onComplete) { }
}
