using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerGameStat
{
    public int sessionNumber;
    public int characterModelNum;
    
    [SerializeField] private PlayerInGameStat playerInGameStat;
    
    public PlayerInGameStat InGameStat => playerInGameStat;
}
