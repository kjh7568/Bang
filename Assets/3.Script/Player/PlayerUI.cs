using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField]private SpriteRenderer spriteRenderer;

    [SerializeField] private HumanList humanList;
    [SerializeField] private Player player; // 직접 Player 참조

    [SerializeField] private List<GameObject> playerHP;

    //private int hp;
    
    public void UpdatePlayerHp()
    {
        int hp = player.SyncPlayerHp;  
        for (int i = 0; i < playerHP.Count; i++)
            playerHP[i].SetActive(i < hp);
    }
    
    private void Start()
    {
        SetPlayerHumanCardSprite();
    }

    private void SetPlayerHumanCardSprite()
    {
        if (player == null)
        {
            Debug.LogError("Player 참조가 비어있습니다.");
            return;
        }
    }
}
