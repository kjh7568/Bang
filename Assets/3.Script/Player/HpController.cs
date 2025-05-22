using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Hpcoins = new List<GameObject>();

    [SerializeField] private Player player;

    private void Update()
    {
        
        
        int hp = player.GameStat.InGameStat.hp;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            hp--;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            hp++;
        }
        
        for (int i = 0; i < Hpcoins.Count; i++)
        {
            // hp보다 작으면 true, 그 이상이면 false
            Hpcoins[i].SetActive(i < hp);
        }
    }
    
    
}
