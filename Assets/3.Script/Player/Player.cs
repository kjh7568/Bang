using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    public PlayerGameStat playerGameStat;
    // [SerializeField] private PlayerBasicStat playerBasicStat;
    //
    // public PlayerGameStat GameStat => playerGameStat;
    // public PlayerBasicStat BasicStat => playerBasicStat;

    public static Player LocalPlayer;
    public static List<Player> ConnectedPlayers = new();

    public PlayerRef playerRef;

    public override void Spawned()
    {
        ConnectedPlayers.Add(this);

        playerRef = Object.InputAuthority;

        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
        }
    }

    public static Player GetPlayer(PlayerRef playerRef)
    {
        return ConnectedPlayers.Find(p => p.playerRef == playerRef);
    }
    
    public static Player GetPlayer(int index)
    {
        return ConnectedPlayers.Find(p => p.playerRef.AsIndex == index);
    }
}