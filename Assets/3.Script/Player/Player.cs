using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerGameStat playerGameStat;
    [SerializeField] private PlayerBasicStat playerBasicStat;
    [SerializeField] private PlayerInGameStat playerInGameStat;
    
    public PlayerGameStat GameStat => playerGameStat;
    public PlayerBasicStat BasicStat => playerBasicStat;
    public PlayerInGameStat InGameStat => playerInGameStat;

    public static Player LocalPlayer;
    public static List<Player> ConnectedPlayers = new();

    public PlayerRef playerRef;

    public override void Spawned()
    {
        var nickname = FindObjectOfType<SavePlayerBasicStat>().Nickname;
        BasicStat.nickName = nickname;
        
        ConnectedPlayers.Add(this);

        playerRef = Object.InputAuthority;

        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
        }

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        ConnectedPlayers.Remove(this);
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