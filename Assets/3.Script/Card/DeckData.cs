using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckData", menuName = "Card/DeckData")]
public class DeckData : ScriptableObject
{
    public List<CardData> cardList;
}
