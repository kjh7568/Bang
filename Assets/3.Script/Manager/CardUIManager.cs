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

    private Player playerStat;
    
    private void Awake()
    {
        Instance = this;
        
        playerStat = GetComponent<Player>();
        Initialize();
    }

    public void SetHandCardImageList()
    {
        handCardImageList = UIManager.Instance.handCardImageList;

        UpdateHandCardUI();
    }
    
    public void UpdateHandCardUI()
    {
        Debug.Log("핸드 카드 UI 업데이트 시작");
        
        for (int i = 0; i < playerStat.GameStat.InGameStat.HandCardsId.Length; i++)
        {
            // so id 값으로 조회해서 스프라이트 변경
            
            Debug.Log($"{playerStat.name} :: {playerStat.GameStat.InGameStat.HandCardsId[i]}");
            
            // if (playerStat.GameStat.InGameStat.HandCards[i] == null) return;
            
            handCardImageList[i].sprite = playerStat.GameStat.InGameStat.HandCards[i].CardSprite;
        }
        
        Debug.Log("핸드 카드 UI 업데이트 완료");
    }
    
    private static Dictionary<int, CardData> idToCard;

    public static void Initialize()
    {
        var cards = Resources.LoadAll<CardData>("Cards"); 
        idToCard = cards.ToDictionary(card => card.CardID, card => card);
    }

    public CardData GetCardByID(int id)
    {
        Debug.Log($"GetCardByID::{id}");
        if (idToCard.TryGetValue(id, out var card))
            return card;

        Debug.LogError($"Card ID {id} not found.");
        return null;
    }
}

// public static class CardDatabase
// {
//
// }
