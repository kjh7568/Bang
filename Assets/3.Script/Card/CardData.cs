using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardTypeEnum;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData")]
public class CardData : ScriptableObject, ICard
{
    [SerializeField] private string name;
    [SerializeField] private int number;
    [SerializeField] private CardType cardType;
    [SerializeField] private CardSymbol cardSymbol;
    
    public string Name => name;
    public int Number => number;
    public CardType CardType => cardType;
    public CardSymbol CardSymbol => cardSymbol;

    public virtual void UseCard()
    {
        // 기능 구현
    }
}
