using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

// public enum DrawType
// {
//     Initial,  // 초기 5장
//     DrawOne,  // 1장 추가
//     Custom    // n장
// }

public class CardSystem : MonoBehaviour
{
     public static CardSystem Instance;
//     
//     //함수명, 멤버 변수 명 등 전부 원하시는대로 바꿔도 됨
     [SerializeField] private DeckData deckData;
//     [SerializeField] private GameObject[] cardPrefab;
//     [SerializeField] private Transform deckParent;

     public List<CardData> initDeck = new List<CardData>();
     public Dictionary<int, CardData> cardByID_Dic;
//     // private List<CardData> usedDeck = new List<CardData>();
//     
//     // 카드 id 초기 리스트
//     List<int> cardToIdList = new List<int>();
//     // 셔플된 초기 카드
//     public List<CardData> initDeck = new List<CardData>();
//     // 사용된 카드
//     private List<CardData> usedDeck = new List<CardData>();
//     
     private void Awake()
     {
         Instance = this;

         InitializeDic();
         // 카드 아이디 컨버팅
         // ConvertCardListToIdList();
     }
     
     public void Init()
     {
         MakeDeck();
         InitDistributeHandCards();
         
         //CardUIManager.Instance.SetHandCardImageList();
     }
     
     private void MakeDeck()
     {
         List<CardData> unShuffledDeck = new List<CardData>(deckData.cardList);
         
         initDeck = unShuffledDeck.OrderBy(x => Random.value).ToList();
     }

     private void InitDistributeHandCards()
     {
         foreach (var player in Player.ConnectedPlayers)
         {
             ICard[] newHand = new ICard[5];
             int[] newHandID = new int[5];
             
             for (int i = 0; i < 3; i++)
             {
                 newHand[i] = initDeck[0];
                 newHandID[i] = initDeck[0].CardID;
                 initDeck.RemoveAt(0);
             }

             for (int i = 3; i < 5; i++)
             {
                 newHand[i] = null;
                 newHandID[i] = 0;
             }
     
             player.InGameStat.HandCards = newHand;
             player.InGameStat.HandCardsId = newHandID;
             
             Broadcaster.Instance.RPC_ReceiveHandCardIDData(player.playerRef, newHandID);
         }
     }
     
     public void InitializeDic()
     {
         if (deckData == null)
         {
             Debug.LogError("deckData가 할당되지 않았습니다!");
             return;
         }

         List<CardData> cards = deckData.cardList;
         cardByID_Dic = cards.ToDictionary(card => card.CardID, card => card);
     }

     public CardData GetCardByIDOrNull(int id)
     {
         if (cardByID_Dic.TryGetValue(id, out CardData card))
             return card;
        
         Debug.Log($"Card ID {id} not found.");
         return null;
     }
     
//     
//     /*
//      void Start()
//      {
//         Vector3 startPos = deckParent.position;
//         int count = cardPrefab.Length;
//
//         for (int i = 0; i < count; i++)
//         {
//             int randNum = Random.Range(0, cardPrefab.Length);
//
//             if (deckObject.Contains(cardPrefab[randNum]))
//             {
//                 i--;
//                 continue;
//             }
//
//             deckObject.Add(Instantiate(cardPrefab[randNum], startPos, Quaternion.Euler(-90f, 0f, 0f), deckParent));
//             startPos += new Vector3(0, 0.01f, 0);
//         }
//      }
//      */
//     
//     public void ConvertCardListToIdList()
//     {
//         foreach (var card in deckData.cardList)
//         {
//             int id = card.CardID;
//             cardToIdList.Add(id);
//         }
//
//         //return cardToIdList;
//     }

}
