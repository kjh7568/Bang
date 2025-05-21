using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardTypeEnum;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance;
    
    [SerializeField] private List<Image> handCardImageList;
    [SerializeField] private DeckData deckData;
    
    //private Player playerStat;
    
    private void Awake()
    {
        Instance = this;
        
        //playerStat = GetComponent<Player>();
        Initialize();
    }

    public void SetHandCardImageList()
    {
        //handCardImageList = UIManager.Instance.handCardImageList;

        //UpdateHandCardUI();
    }
    
    public void UpdateHandCardUI(ICard[] cards)
    {
        Debug.Log("핸드 카드 UI 업데이트 시작");
        Debug.Log($"cards.Length:: {cards.Length}");
        for (int i = 0; i < cards.Length; i++)
        {
            // so id 값으로 조회해서 스프라이트 변경
            
            Debug.Log($" {cards[i]}");
            
            // if (playerStat.GameStat.InGameStat.HandCards[i] == null) return;
            
            handCardImageList[i].sprite = cards[i].CardSprite;
        }
        
        //Debug.Log("핸드 카드 UI 업데이트 완료");
    }
    
    private static Dictionary<int, CardData> idToCard;

    public void Initialize()
    {
        if (deckData == null)
        {
            Debug.LogError("deckData가 할당되지 않았습니다!");
            return;
        }

        List<CardData> cards = deckData.cardList;
        idToCard = cards.ToDictionary(card => card.CardID, card => card);

        Debug.Log($"[Initialize] 카드 총 개수: {idToCard.Count}");

        foreach (var card in cards)
        {
            Debug.Log($"[Card] ID: {card.CardID}, Name: {card.Name}");
        }
    }

    public CardData GetCardByID(int id)
    {
        //Debug.Log($"GetCardByID::{id}");
        if (idToCard.TryGetValue(id, out CardData card))
            return card;

        Debug.LogError($"Card ID {id} not found.");
        return null;
    }
}

// public static class CardDatabase
// {
//
// }
