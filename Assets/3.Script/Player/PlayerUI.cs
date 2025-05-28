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

    //[SerializeField] private List<GameObject> Hpcoins = new List<GameObject>();
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

    // public override void FixedUpdateNetwork()
    // {
    //     base.FixedUpdateNetwork();
    //
    //     //UpdatePlayerHp();
    // }

    private void SetPlayerHumanCardSprite()
    {
        if (player == null)
        {
            Debug.LogError("Player 참조가 비어있습니다.");
            return;
        }

        // string humanName = player.GameStat.InGameStat.MyHuman.Name;
        //
        // foreach (var humanData in humanList.humanList)
        // {
        //     if (humanData.Name == humanName)
        //     if (humanData.Name == humanName)
        //     {
        //         spriteRenderer.sprite = humanData.CardSprite;
        //         RPC_SetPlayerHumanCardSprite(humanName);
        //         break;
        //     }
        // }
    }
    
    // public void UpdatePlayerHp()
    // {
    //     hp = Player.LocalPlayer.InGameStat.hp;
    //     
    //     for (int i = 0; i < playerHP.Count; i++)
    //     {
    //         playerHP[i].SetActive(i < hp);
    //     }
    // }
    
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_SetPlayerHumanCardSprite(string humanName)
    {
        foreach (var humanData in humanList.humanList)
        {
            if (humanData.Name == humanName)
            {
                spriteRenderer.sprite = humanData.CardSprite;
                break;
            }
        }
    }
}
