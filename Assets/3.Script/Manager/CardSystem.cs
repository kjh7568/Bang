using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

public class CardSystem : MonoBehaviour
{
    //함수명, 멤버 변수 명 등 전부 원하시는대로 바꿔도 됨
    
    [SerializeField]private GameObject[] cardPrefab;
    [SerializeField]private Transform deckParent;

    private List<GameObject> deckObject = new List<GameObject>();

    public void Init()
    {
        MakeDeck();
        DistributeHandCards();
    }
    
    //게임 시스템과 마찬가지로 본격적으로 구현시작하면 서버와 연동해서 작업해야 함.
    //덱 정보를 만들고 섞는 건 서버만 알고 있어도 되나 카드를 넘겨주는 함수 같은 경우는 RPC를 이용해서 정보를 넘겨줘야 함 --> 이건 차후 잘 해봐야 할 듯
    public void ShuffleDeck()
    {
        Debug.Log("덱 셔플");
    }

    //요고 RPC 써야 할 듯
    public void DistributeHandCards()
    {
        Debug.Log("핸드 카드 분배");
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
}
