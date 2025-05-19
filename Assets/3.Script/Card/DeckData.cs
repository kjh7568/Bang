using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckData", menuName = "Card/DeckData")]
public class DeckData : ScriptableObject
{
    public List<CardData> cardList;
    private Dictionary<int, CardData> cardDictionary;

    public void Initialize()
    {
        cardDictionary = new Dictionary<int, CardData>();
        foreach (var card in cardList)
        {
            if (card != null && !cardDictionary.ContainsKey(card.CardID))
            {
                cardDictionary.Add(card.CardID, card);
            }
        }
    }

    public CardData GetCardById(int cardId)
    {
        if (cardDictionary.TryGetValue(cardId, out CardData card))
        {
            return card;
        }

        return null;
    }
}
