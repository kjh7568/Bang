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

     public Dictionary<string, Action<PlayerRef, PlayerRef?>> actionByName_Dic;
     
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
             int[] newHandID = new int[5];
             
             for (int i = 0; i < 3; i++)
             {
                 newHandID[i] = initDeck[0].CardID;
                 initDeck.RemoveAt(0);
             }

             for (int i = 3; i < 5; i++)
             {
                 newHandID[i] = 0;
                 // Broadcaster.Instance.RPC_OnAndOffCardButton(player.playerRef, false, i);
             }
     
             player.InGameStat.HandCardsId = newHandID;
             
             Broadcaster.Instance.RPC_ReceiveHandCardAndUpdateUi(player.playerRef, newHandID);
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
         
         actionByName_Dic = new Dictionary<string, Action<PlayerRef, PlayerRef?>>()
         {
             { "Bang", UseBang },
             { "Missed", UseMissed },
             { "Beer", UseBeer }
         };
     }
     
     public CardData GetCardByIDOrNull(int id)
     {
         if (cardByID_Dic.TryGetValue(id, out CardData card))
             return card;
        
         Debug.Log($"Card ID {id} not found.");
         return null;
     }

     public bool CheckHasMissed(PlayerRef playerRef)
     {
         var cardID = Player.GetPlayer(playerRef).InGameStat.HandCardsId;
         
         for (int i = 0; i < cardID.Length; i++)
         {
             if (cardID[i] == 0) continue;
             
             var card = GetCardByIDOrNull(cardID[i]);

             if (card.Name == "Missed")
             {
                 return true;
             }
         }
         
         return false;
     }
     
     public void DoActionByName(string cardName, PlayerRef user, PlayerRef? target = null)
     {
         if (actionByName_Dic.TryGetValue(cardName, out var action) == false)
         {
             Debug.LogWarning($"등록되지 않은 카드 이름: {cardName}");
             UseAnyCard(user, target);
             return;
         }

         action.Invoke(user, target);
     }
     
     private void UseBang(PlayerRef user, PlayerRef? target)
     {
         if (target == null)
         {
             Debug.LogWarning("뱅 카드는 대상이 필요합니다!");
             return;
         }

         Broadcaster.Instance.RPC_RequestBang(user, target.Value);
     }

     private void UseMissed(PlayerRef user, PlayerRef? target)
     {
         Debug.Log($"{user}가 빗나감을 사용함");
     }

     private void UseBeer(PlayerRef user, PlayerRef? target)
     {
         if (Player.GetPlayer(user).InGameStat.hp >= 5)
         {
             Debug.Log("풀피인데 맥주를 먹으면 안되죠...");
             return;
         }
         
         Player.GetPlayer(user).InGameStat.hp++;
         Broadcaster.Instance.RPC_NotifyBeer(user);
         // 예: PlayerManager.Instance.Heal(user, 1);
     }
     
     private void UseAnyCard(PlayerRef user, PlayerRef? target)
     {
         Debug.Log($"{user}가 아무 카드를 사용");
         UIManager.Instance.cardListPanel.SetActive(true);
         // 예: PlayerManager.Instance.Heal(user, 1);
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
