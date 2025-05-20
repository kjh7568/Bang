using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

public class CardSystem : MonoBehaviour
{
    //함수명, 멤버 변수 명 등 전부 원하시는대로 바꿔도 됨
    [SerializeField] private DeckData deckData;
    [SerializeField] private GameObject[] cardPrefab;
    [SerializeField] private Transform deckParent;

    // private List<CardData> initDeck = new List<CardData>();
    // private List<CardData> usedDeck = new List<CardData>();
    
    // 카드 id 초기 리스트
    List<int> cardToIdList = new List<int>();
    // 셔플된 초기 카드
    private List<int> initDeck = new List<int>();
    // 사용된 카드
    private List<int> usedDeck = new List<int>();
    
    private void Awake()
    {
        // 카드 아이디 컨버팅
        ConvertCardListToIdList();
    }

    public void Init()
    {
        MakeDeck();
        DistributeHandCards();
        
        //CardUIManager.Instance.SetHandCardImageList();
    }
    
    //게임 시스템과 마찬가지로 본격적으로 구현시작하면 서버와 연동해서 작업해야 함.
    //덱 정보를 만들고 섞는 건 서버만 알고 있어도 되나 카드를 넘겨주는 함수 같은 경우는 RPC를 이용해서 정보를 넘겨줘야 함 --> 이건 차후 잘 해봐야 할 듯
    public void ShuffleDeck()
    {
        initDeck = cardToIdList.OrderBy(x => Random.value).ToList();

        Debug.Log("덱 셔플 완료");
    }

    //요고 RPC 써야 할 듯
    public void DistributeHandCards()
    {
        Debug.Log("핸드 카드 분배");

        foreach (var player in GameManager.Instance.players)
        {
            int[] hand = new int[5];

            for (int i = 0; i <= 4; i++)
            {
                hand[i] = initDeck[0];
                initDeck.RemoveAt(0);
            }

            player.GameStat.InGameStat.HandCardsId = hand;

            // 호스트는 직접 카드 객체 생성
            var cards = new CardData[5];
            for (int i = 0; i < hand.Length; i++)
            {
                cards[i] = CardUIManager.Instance.GetCardByID(hand[i]);
            }
            player.GameStat.InGameStat.HandCards = cards;

            // 카드 ID만 보내서 클라이언트에서 생성하게 함
            player.RPC_ReceiveToHandCardsData(hand);
        }
    }


    // public void DistributeHandCards()
    // {
    //     Debug.Log("핸드 카드 분배");
    //
    //     foreach (var player in GameManager.Instance.players)
    //     {
    //         for (int i = 0; i < 2; i++)
    //         {
    //             // player.GameStat.InGameStat.HandCards[i] = initDeck[i];
    //             // initDeck.RemoveAt(i);
    //             
    //             player.GameStat.InGameStat.HandCards[i] = initDeck[0];
    //             initDeck.RemoveAt(0);
    //             
    //             Debug.Log($"{player.BasicStat.nickName} 핸드 카드 ::: {player.GameStat.InGameStat.HandCards[i]}");
    //         }
    //     }
    // }

    private void UseTest()
    {
        
    }
    
    private void MakeDeck()
    {
        Debug.Log("덱 생성");

        //덱 생성 후 조기 셔플 --> 혹은 생성과 동시에 셔플한 채로 만들어도 되긴 함
        ShuffleDeck();
    }
    /*
     void Start()
     {
        Vector3 startPos = deckParent.position;
        int count = cardPrefab.Length;

        for (int i = 0; i < count; i++)
        {
            int randNum = Random.Range(0, cardPrefab.Length);

            if (deckObject.Contains(cardPrefab[randNum]))
            {
                i--;
                continue;
            }

            deckObject.Add(Instantiate(cardPrefab[randNum], startPos, Quaternion.Euler(-90f, 0f, 0f), deckParent));
            startPos += new Vector3(0, 0.01f, 0);
        }
     }
     */
    
    public void ConvertCardListToIdList()
    {
        foreach (var card in deckData.cardList)
        {
            int id = card.CardID;
            cardToIdList.Add(id);
        }

        //return cardToIdList;
    }

}
