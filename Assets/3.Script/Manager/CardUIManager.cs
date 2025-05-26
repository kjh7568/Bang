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
        
        Initialize();
    }
    
    public void UpdateHandCardUI(int[] cards)
    {
        //Debug.Log("핸드 카드 UI 업데이트 시작");
        //Debug.Log($"cards.Length:: {cards.Length}");
        for (int i = 0; i < cards.Length; i++)
        {
            // so id 값으로 조회해서 스프라이트 변경

            if (cards[i] == 0)
            {
                continue;    
            }
            
            Debug.Log($" {cards[i]}");
            
            handCardImageList[i].sprite = GetCardByIDOrNull(cards[i]).CardSprite;
        }
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

        //Debug.Log($"[Initialize] 카드 총 개수: {idToCard.Count}");
    }

    public CardData GetCardByIDOrNull(int id)
    {
        if (idToCard.TryGetValue(id, out CardData card))
            return card;
        
        Debug.Log($"Card ID {id} not found.");
        return null;
    }
}

