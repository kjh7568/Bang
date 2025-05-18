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
        Debug.Log($"플레이어 정보 ::: {playerStat.BasicStat.nickName}");
    }

    public void SetHandCardImageList()
    {
        handCardImageList = UIManager.Instance.handCardImageList;

        for (int i = 0; i < handCardImageList.Count; i++)
        {
            Debug.Log($"핸드 카드 UI 설정 완료 ::: {handCardImageList[i].name}");
        }

        UpdateHandCardUI();
    }
    
    public void UpdateHandCardUI()
    {
        Debug.Log("핸드 카드 UI 업데이트 시작");
        
        for (int i = 0; i < playerStat.GameStat.InGameStat.HandCards.Length; i++)
        {
            Debug.Log($"핸드 카드 확인 ::: {playerStat.GameStat.InGameStat.HandCards[i].Name}");
            
            if (playerStat.GameStat.InGameStat.HandCards[i] == null) return;
            
            handCardImageList[i].sprite = playerStat.GameStat.InGameStat.HandCards[i].CardSprite;
        }
        
        Debug.Log("핸드 카드 UI 업데이트 완료");
    }
}
