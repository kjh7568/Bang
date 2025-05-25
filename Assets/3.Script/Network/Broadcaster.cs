using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;

    [Networked] public int TurnIndex { get; set; }
    public PlayerRef[] syncedPlayerRefs;
    public Player[] syncedPlayerClass;

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

        GameManager.Instance.SetLocalPlayer(syncedPlayerRefs);
        UIManager.Instance.SetTargetSelectionUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetPanel()
    {
        UIManager.Instance.waitingPanel.SetActive(false);
        UIManager.Instance.cardListPanel.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AttackPlayerNotify(PlayerRef localRef, PlayerRef targetRef)
    {

        // 여기부턴 지정당한 사람이 해야할 행동 로직
        // 1. 일단 빗나감이 있는지 확인
        RPC_SearchMissed(localRef, targetRef);
        // 2. 빗나감에 사용 유무를 묻는 패널 On
        // 2-1. 빗나감이 없으면 Yes 버튼이 비활성화
        // 3. 빗나감을 사용하지 않으면 컴벳 시스템을 통해 데미지 처리
        
        // 3-1. 빗나감을 썼다면 모두에게 알림을 띄우고 다시 LocalRef의 카드 패널을 킴
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SearchMissed(PlayerRef localRef, PlayerRef targetRef)
    {
        var hand = BasicSpawner.Instance.spawnedPlayers[targetRef].GetComponent<Player>().GameStat.InGameStat.HandCards;
        
        bool found = hand.Any(c => c != null && c.Name == "Missed");

        Debug.Log(found);
        RPC_OpenUseMissedPanel(found, localRef, targetRef);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OpenUseMissedPanel(bool hasMissed, PlayerRef attackPlayerRef, PlayerRef targetPlayerRef)
    {
        if (BasicSpawner.Instance._runner.LocalPlayer != targetPlayerRef) return;

        UIManager.Instance.ShowMissedPanel(hasMissed, attackPlayerRef, targetPlayerRef);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastMissedUsage(bool useMissed, PlayerRef localRef, PlayerRef targetRef)
    {
        var player = GetPlayer(targetRef);

        if (useMissed)
        {
            Debug.Log($"[게임 로그] {player.BasicStat.nickName}님이 '빗나감(Missed)' 카드를 사용했습니다!");
        }
        else
        {
            Debug.Log($"[게임 로그] {player.BasicStat.nickName}님이 '빗나감(Missed)' 카드를 사용하지 못하여 데미지를 받았습니다!");
        }
        
        if (BasicSpawner.Instance._runner.LocalPlayer != localRef) return;

        UIManager.Instance.cardListPanel.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DrawCard(PlayerRef playerRef, int addCount)
    {
        Debug.Log($"{playerRef.ToString()} 덱에 카드 {addCount}장 추가");
        
        int nullCount = 0;
        List<int> nullIndexes = new List<int>();
        
        var playerComponent = BasicSpawner.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
        var handCards = playerComponent.GameStat.InGameStat.HandCards;
        var handCardsId = playerComponent.GameStat.InGameStat.HandCardsId;
        
        for (int i = 0; i < handCards.Length; i++)
        {
            if (handCards[i] == null)
            {
                nullIndexes.Add(i);
                nullCount++;
            }
        }

        while (nullCount > 0)
        {
            for (int i = 0; i < addCount; i++)
            {
                handCards[nullIndexes[i]] = CardSystem.Instance.initDeck[0];
                handCardsId[nullIndexes[i]] = CardSystem.Instance.initDeck[0].CardID;
                RPC_OnCardButton(playerRef, nullIndexes[i]);
                CardSystem.Instance.initDeck.RemoveAt(0);

                nullCount--;
            }
        }
        
        playerComponent.RPC_ReceiveToHandCardsData(handCardsId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnCardButton(PlayerRef playerRef, int index)
    {
        if (BasicSpawner.Instance._runner.LocalPlayer != playerRef) return;
        
        UseCardUI.Instance.cardButtons[index].SetActive(true);
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
        for (int i = 0; i < syncedPlayerClass.Length; i++)
        {
            if (syncedPlayerRefs[i] == playerRef)
                return syncedPlayerClass[i];
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

        var playerComponent = BasicSpawner.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
        var cards = playerComponent.GameStat.InGameStat.HandCards;

        if (cards[cardIdx].IsTargetRequired)
        {
            RPC_ShowPlayerSelectPanel(playerRef);
            cards[cardIdx].UseCard(cardIdx);
            cards[cardIdx] = null;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ShowPlayerSelectPanel(PlayerRef playerRef)
    {
        if (BasicSpawner.Instance._runner.LocalPlayer != playerRef) return;

        UIManager.Instance.ShowPlayerSelectPanel();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_MakeCombatEvent(PlayerRef senderRef, PlayerRef targetRef, int damage, RpcInfo info = default)
    {
        Debug.Log($"호출 주체 {senderRef}");

        var target = InGameSystem.Instance.GetPlayerOrNull(targetRef);

        if (target != null)
        {
            CombatEvent combatEvent = new CombatEvent
            {
                Sender = BasicSpawner.Instance.spawnedPlayers[senderRef].GetComponent<Player>().GameStat.InGameStat,
                Receiver = target,
                Damage = damage
            };
            
            InGameSystem.Instance.AddInGameEvent(combatEvent);
            
            //RPC로 체력 변경 사항 보내주기
        }
        else
        {
            Debug.LogWarning("타겟이 없습니다.");
        }
    }
}