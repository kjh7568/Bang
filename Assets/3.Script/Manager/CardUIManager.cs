using System;
using System.Collections;
using System.Collections.Generic;
using CardTypeEnum;
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
            
            
            // if (playerStat.GameStat.InGameStat.HandCards[i] == null) return;
            //
            // handCardImageList[i].sprite = playerStat.GameStat.InGameStat.HandCards[i].CardSprite;
        }
        
        Debug.Log("핸드 카드 UI 업데이트 완료");
    }
}
