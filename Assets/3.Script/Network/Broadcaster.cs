using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;

    [Networked] public int TurnIndex { get; set; }
    
    public PlayerRef[] allPlayerRefs;
    public Player[] allPlayerClass;

    public Player LocalPlayer;
    public PlayerRef LocalRef;
    
    private void Awake()
    {
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateNicknames(string[] nicknames)
    {
        WatingSetting ui = FindObjectOfType<WatingSetting>();
        if (ui != null)
            ui.UpdateNicknameTexts(nicknames);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendNicknameToHost(string nickname, RpcInfo info = default)
    {
        PlayerRef playerRef = info.Source;

        // 닉네임을 BasicSpawner에서 갱신
        if (Server.Instance != null)
        {
            Server.Instance.ReceiveNicknameFromClient(playerRef, nickname);
        }
        else
        {
            Debug.LogWarning("[Broadcaster] BasicSpawner 인스턴스를 찾을 수 없습니다.");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncSpawnedPlayers(PlayerRef[] playerRefs, Player[] playerClass)
    {
        allPlayerRefs = new PlayerRef[playerRefs.Length];
        allPlayerClass = new Player[playerClass.Length];

        allPlayerRefs = playerRefs;
        allPlayerClass = playerClass;
        
        Debug.Log($"Received {playerRefs.Length} playerRefs");
        Debug.Log($"Received {playerClass.Length} playerClass");

        SetLocalPlayer(allPlayerRefs);
        UIManager.Instance.SetTargetSelectionUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetPanel()
    {
        UIManager.Instance.waitingPanel.SetActive(false);
        UIManager.Instance.cardListPanel.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AttackPlayerNotify(PlayerRef localRef, PlayerRef targetRef)
    {
        Debug.Log($"local::{localRef}");
        Debug.Log($"target::{targetRef}");
        
        var local = GetPlayer(localRef);
        var target = GetPlayer(targetRef);
        
        Debug.Log($"{local.BasicStat.nickName}님이 {target.BasicStat.nickName}을(를) 공격 대상으로 선택함");

        if (targetRef == Runner.LocalPlayer)
        {
            Debug.Log($"{target.BasicStat.nickName}님의 카드선택");

            UIManager.Instance.ShowCardSelectionPanel((selectedCardID) =>
            {
                RPC_TargetSelectedCard(localRef, targetRef, selectedCardID);
            });
        }
        else if (localRef == Runner.LocalPlayer)
        {
            UIManager.Instance.ShowWaitingForTargetPanel();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TargetSelectedCard(PlayerRef attacker, PlayerRef target, int cardIndex)
    {
        Debug.Log($"attacker:: {attacker}");
        Debug.Log($"target:: {target}");
        Debug.Log($"cardId:: {cardIndex}");

        if (Runner.LocalPlayer != attacker) return; 
        var attackPlayer = GetPlayer(attacker);
        var targetPlayer = GetPlayer(target);
        
        Debug.Log($"targetPlayer:: {targetPlayer.GameStat.InGameStat.HandCards[cardIndex]}");
        
        var selectedCard = targetPlayer.GameStat.InGameStat.HandCards[cardIndex];
        
        Debug.Log($"selectedCard:: {selectedCard}");
        
        if (targetPlayer != null)
        {
            Debug.Log($"{attackPlayer.BasicStat.nickName}님의 공격이 끝났습니다.");
            
            Debug.Log($"{targetPlayer.BasicStat.nickName}님이 {selectedCard.Name}, {selectedCard.CardID} 카드를 선택함");
        }
        else
        {
            Debug.LogWarning("Target Player를 찾을 수 없습니다.");
        }

        TurnManager.Instance.ContinueTurn(attacker);
    }
    
    public Player GetPlayer(PlayerRef playerRef)
    {
        for (int i = 0; i < allPlayerClass.Length; i++)
        {
            if (allPlayerRefs[i] == playerRef)
                return allPlayerClass[i];
        }

        return null;
    }
    
    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void RPC_SetLocalPlayerRef(PlayerRef refToSet)
    // {
    //     LocalRef = refToSet;
    //     Debug.Log($"[서버] Broadcaster에 LocalRef 저장: {refToSet}");
    // }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_ShowResult(string result, string[] playerInfos)
    {
        var victoryCheck = FindObjectOfType<VictoryCheck>();

        victoryCheck.gameResult = result;
        victoryCheck.OpenGameResultUI(playerInfos);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUseCardList(PlayerRef playerRef, int cardIdx)
    {
        Debug.Log($"{playerRef} 클라이언트 → 카드 사용 요청");
        Debug.Log($"전달된 카드 Number: {cardIdx}");

        var playerComponent = Server.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
        var card = playerComponent.GameStat.InGameStat.HandCards[cardIdx];
        
        card.UseCard(() => {
            Debug.Log("카드 효과 완료 → 다음 카드 선택 패널 표시");
        });
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_MakeCombatEvent(PlayerRef senderRef, PlayerRef targetRef, int damage, RpcInfo info = default)
    {
        // Debug.Log($"호출 주체 {senderRef}");
        //
        // // var target = InGameSystem.Instance.GetPlayerOrNull(targetRef);
        // var target = InGameSystem.Instance.GetPlayerOrNull(BasicSpawner.Instance._runner.LocalPlayer);
        //
        // CombatEvent combatEvent = new CombatEvent
        // {
        //     Sender = BasicSpawner.Instance.spawnedPlayers[senderRef].GetComponent<Player>().GameStat.InGameStat,
        //     Receiver = target,
        //     Damage = damage
        // };
        //
        // InGameSystem.Instance.AddInGameEvent(combatEvent);
    }
    
    /*=============================================================================================================*/
    
    public void SetLocalPlayer(PlayerRef[] playerRefs)
    {
        Debug.Log($"SetLocalPlayer 실행");

        foreach (var playerRef in playerRefs)
        {
            Debug.Log($"SetLocalPlayer ::{Broadcaster.Instance.LocalRef}");
           
            if (Server.Instance.spawnedPlayers.TryGetValue(playerRef, out var obj))
            {
                var player = obj.GetComponent<Player>();

                Broadcaster.Instance.LocalPlayer = player;
                Broadcaster.Instance.LocalRef = playerRef;
                UIManager.Instance.localPlayer = playerRef;

                Debug.Log($"내 플레이어 설정 완료: {player.BasicStat.nickName}");
            }
            else
            {
                Debug.LogWarning($"[SetLocalPlayer] spawnedPlayers에 {playerRef}가 없습니다.");
            }

            break; // 찾았으니까 루프 종료
            
            // if (playerRef == Broadcaster.Instance.LocalRef)
            // {
            //
            // }
        }

        LocalRef = Server.Instance._runner.LocalPlayer;
        LocalPlayer = allPlayerClass[allPlayerRefs.ToList().IndexOf(LocalRef)];
    }
}