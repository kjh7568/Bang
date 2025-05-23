using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;

    [Networked] public int TurnIndex { get; set; }
    public PlayerRef[] syncedPlayerRefs;
    public Player[] syncedPlayerClass;
    public Player LocalPlayer;

    private NetworkRunner networkRunner;

    private void Awake()
    {
        Instance = this;
        networkRunner = FindObjectOfType<NetworkRunner>();

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
        if (BasicSpawner.Instance != null)
        {
            BasicSpawner.Instance.ReceiveNicknameFromClient(playerRef, nickname);
        }
        else
        {
            Debug.LogWarning("[Broadcaster] BasicSpawner 인스턴스를 찾을 수 없습니다.");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncSpawnedPlayers(PlayerRef[] playerRefs, Player[] playerClass)
    {
        syncedPlayerRefs = new PlayerRef[playerRefs.Length];
        syncedPlayerClass = new Player[playerClass.Length];

        syncedPlayerClass = playerClass;
        syncedPlayerRefs = playerRefs;

        Debug.Log($"Received {playerRefs.Length} playerRefs");
        Debug.Log($"Received {playerClass.Length} playerClass");

        UIManager.Instance.SetTargetSelectionUI();
        GameManager.Instance.SetLocalPlayer(syncedPlayerRefs);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetPanel()
    {
        UIManager.Instance.waitingPanel.SetActive(false);
        UIManager.Instance.cardListPanel.SetActive(false);
    }

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

        var playerComponent = BasicSpawner.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
        var card = playerComponent.GameStat.InGameStat.HandCards[cardIdx];
        
        card.UseCard(() => {
            Debug.Log("카드 효과 완료 → 다음 카드 선택 패널 표시");
        });
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_MakeCombatEvent(PlayerRef senderRef, PlayerRef targetRef, int damage, RpcInfo info = default)
    {
        Debug.Log($"호출 주체 {senderRef}");

        // var target = InGameSystem.Instance.GetPlayerOrNull(targetRef);
        var target = InGameSystem.Instance.GetPlayerOrNull(BasicSpawner.Instance._runner.LocalPlayer);

        CombatEvent combatEvent = new CombatEvent
        {
            Sender = BasicSpawner.Instance.spawnedPlayers[senderRef].GetComponent<Player>().GameStat.InGameStat,
            Receiver = target,
            Damage = damage
        };

        InGameSystem.Instance.AddInGameEvent(combatEvent);
    }
}