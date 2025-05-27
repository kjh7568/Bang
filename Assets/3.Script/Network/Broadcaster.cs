using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;

//
//     [Networked] public int TurnIndex { get; set; }
//     public PlayerRef[] syncedPlayerRefs;
//     public Player[] syncedPlayerClass;
//
//     public Player LocalPlayer;
//     public PlayerRef LocalRef;
//
    public int turnIdx = 1;

    public override void Spawned()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartPlayerTurn(PlayerRef playerRef)
    {
        UIManager.Instance.ResetPanel();

        if (Runner.IsServer)
        {
            DrawCard(playerRef);
            RPC_ReceiveHandCardAndUpdateUi(playerRef, Player.GetPlayer(playerRef).InGameStat.HandCardsId);
        }

        if (Runner.LocalPlayer == playerRef)
        {
            UIManager.Instance.cardListPanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }

    private void DrawCard(PlayerRef playerRef)
    {
        int drawCardId = CardSystem.Instance.initDeck[0].CardID;

        var handCardID = Player.GetPlayer(playerRef).InGameStat.HandCardsId;
        
        for (int i = 0; i < handCardID.Length; i++)
        {
            if (handCardID[i] == 0)
            {
                Player.GetPlayer(playerRef).InGameStat.HandCardsId[i] = drawCardId;
                RPC_OnAndOffCardButton(playerRef, true, i);
                CardSystem.Instance.initDeck.RemoveAt(0);
                break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestEndTurn()
    {
        turnIdx = turnIdx % Player.ConnectedPlayers.Count + 1;

        var nextPlayer = Player.GetPlayer(turnIdx).playerRef;

        RPC_StartPlayerTurn(nextPlayer);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveHandCardAndUpdateUi(PlayerRef playerRef, int[] handCardIds)
    {
        if (Runner.LocalPlayer == playerRef)
        {
            Player.GetPlayer(playerRef).InGameStat.HandCardsId = handCardIds;

            UIManager.Instance.UpdateHandCardUI(handCardIds);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUseCard(PlayerRef playerRef, int cardIdx)
    {
        Debug.Log($"{playerRef} 클라이언트 → 카드 사용 요청");
        Debug.Log($"전달된 카드 Number: {cardIdx}");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendPlayerHuman(PlayerRef playerRef, int humanIdx)
    {
        if (Runner.IsServer && Runner.LocalPlayer != playerRef)
        {
            Player.GetPlayer(playerRef).InGameStat.MyHuman = GameManager.Instance.humanList.humanList[humanIdx];
        }
        else if (Runner.LocalPlayer == playerRef)
        {
            Player.LocalPlayer.InGameStat.MyHuman = GameManager.Instance.humanList.humanList[humanIdx];
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendPlayerJob(PlayerRef playerRef, int jobIdx)
    {
        if (Runner.IsServer && Runner.LocalPlayer != playerRef)
        {
            Player.GetPlayer(playerRef).InGameStat.MyJob = GameManager.Instance.jobList.jobList[jobIdx];
        }
        else if (Runner.LocalPlayer == playerRef)
        {
            Player.LocalPlayer.InGameStat.MyJob = GameManager.Instance.jobList.jobList[jobIdx];
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendMyCardId2Server(PlayerRef playerRef, int[] cardId)
    {
        Debug.Log($"{playerRef}가 턴을 넘겼고 현재 패 상태는 {string.Join(", ", cardId)}");
        Player.GetPlayer(playerRef).InGameStat.HandCardsId = cardId;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnAndOffCardButton(PlayerRef playerRef, bool isOn, int buttonIdx)
    {
        if (Runner.LocalPlayer != playerRef) return;

        UIManager.Instance.cardButtons[buttonIdx].SetActive(isOn);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_EndLoading(RpcInfo info = default)
    {
        GameManager.Instance.EndLoading();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RequestBang(PlayerRef attackRef, PlayerRef targetRef)
    {
        Debug.Log($"{attackRef}가 {targetRef}에게 뱅을 사용함");
        Debug.Log($"Runner.LocalPlayer ::: {Runner.LocalPlayer}");
        Debug.Log($"attackRef ::: {attackRef}");
        Debug.Log($"targetRef ::: {targetRef}");

        if (Runner.LocalPlayer == targetRef)
        {
            var hasMissed = CardSystem.Instance.CheckHasMissed(targetRef);
            
            UIManager.Instance.ResetPanel();
            UIManager.Instance.ShowMissedPanel(hasMissed, attackRef, targetRef);
        }
        else
        {
            UIManager.Instance.ResetPanel();
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyMissed(PlayerRef attackRef, PlayerRef targetRef)
    {
        Debug.Log($"{targetRef}가 {attackRef}의 뱅을 빗나감으로 회피하였습니다!");
        Debug.Log($"Runner.LocalPlayer ::: {Runner.LocalPlayer}");
        Debug.Log($"attackRef ::: {attackRef}");
        Debug.Log($"targetRef ::: {targetRef}");
        
        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.ResetPanel();
            UIManager.Instance.cardListPanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.ResetPanel();
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyBang(PlayerRef attackRef, PlayerRef targetRef)
    {
        Debug.Log($"{attackRef}가 {targetRef}에게 뱅을 사용하여 1 데미지를 입혔습니다!");

        if (Runner.IsServer && Runner.LocalPlayer != targetRef)
        {
            Player.GetPlayer(targetRef).InGameStat.hp--;
        }
        
        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.ResetPanel();
            UIManager.Instance.cardListPanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.ResetPanel();
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyBeer(PlayerRef playerRef)
    {
        Debug.Log($"{playerRef}가 맥주를 사용하여 체력 1을 회복했습니다!");

        if (Runner.IsServer && Runner.LocalPlayer != playerRef)
        {
            Player.GetPlayer(playerRef).InGameStat.hp += 1;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_VictoryCheck(PlayerRef playerRef)
    {
        List<Player> players = new List<Player>(Player.ConnectedPlayers);
        string result = "not victory yet";
        
        bool sheriffAlive = players.Any(p => 
            p.InGameStat != null &&
            p.InGameStat.MyJob != null &&
            p.InGameStat.MyJob.Name == "보안관" &&
            !p.InGameStat.IsDead);

        bool renegadeAlive = players.Any(p => 
            p.InGameStat != null &&
            p.InGameStat.MyJob != null &&
            p.InGameStat.MyJob.Name == "배신자" &&
            !p.InGameStat.IsDead);

        int outlawAlive = players.Count(p => 
            p.InGameStat != null &&
            p.InGameStat.MyJob != null &&
            p.InGameStat.MyJob.Name == "무법자" &&
            !p.InGameStat.IsDead);

        foreach (var p in players)
        {
            if (p.InGameStat == null)
            {
                Debug.LogWarning("InGameStat이 null입니다: " + p);
                continue;
            }
    
            if (p.InGameStat.MyJob == null)
            {
                Debug.LogWarning("MyJob이 null입니다: " + p);
                continue;
            }
        }
        
        if (!sheriffAlive)
        {
            if (outlawAlive > 0)
            {
                Debug.Log("무법자 승리!");
                result = "outlawAlive win!";
                RPC_ShowResultToClients(result);
                return;
            }
            else if (renegadeAlive)
            {
                Debug.Log("배신자 승리!");
                result = "outlawAlive win!";
                RPC_ShowResultToClients(result);
                return;
            }
        }
        else if (outlawAlive == 0 && !renegadeAlive)
        {
            Debug.Log("보안관 승리!");
            result = "sheriff win!";
            RPC_ShowResultToClients(result);
            return;
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowResultToClients(string result)
    {
        UIManager.Instance.ShowResultPanel(result);
    }
    
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void RPC_UpdateNicknames(string[] nicknames)
//     {
//         WatingSetting ui = FindObjectOfType<WatingSetting>();
//         if (ui != null)
//             ui.UpdateNicknameTexts(nicknames);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_SendNicknameToHost(string nickname, RpcInfo info = default)
//     {
//         PlayerRef playerRef = info.Source;
//
//         // 닉네임을 BasicSpawner에서 갱신
//         if (Server.Instance != null)
//         {
//             Server.Instance.ReceiveNicknameFromClient(playerRef, nickname);
//         }
//         else
//         {
//             Debug.LogWarning("[Broadcaster] BasicSpawner 인스턴스를 찾을 수 없습니다.");
//         }
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void RPC_SyncSpawnedPlayers(PlayerRef[] playerRefs, Player[] playerClass)
//     {
//         syncedPlayerRefs = new PlayerRef[playerRefs.Length];
//         syncedPlayerClass = new Player[playerClass.Length];
//
//         syncedPlayerClass = playerClass;
//         syncedPlayerRefs = playerRefs;
//
//         Debug.Log($"Received {playerRefs.Length} playerRefs");
//         Debug.Log($"Received {playerClass.Length} playerClass");
//
//         GameManager.Instance.SetLocalPlayer(syncedPlayerRefs);
//         UIManager.Instance.SetTargetSelectionUI();
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void RPC_ResetPanel()
//     {
//         UIManager.Instance.waitingPanel.SetActive(false);
//         UIManager.Instance.cardListPanel.SetActive(false);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_AttackPlayerNotify(PlayerRef localRef, PlayerRef targetRef)
//     {
//         // 여기부턴 지정당한 사람이 해야할 행동 로직
//         // 1. 일단 빗나감이 있는지 확인
//         RPC_SearchMissed(localRef, targetRef);
//         // 2. 빗나감에 사용 유무를 묻는 패널 On
//         // 2-1. 빗나감이 없으면 Yes 버튼이 비활성화
//         // 3. 빗나감을 사용하지 않으면 컴벳 시스템을 통해 데미지 처리
//
//         // 3-1. 빗나감을 썼다면 모두에게 알림을 띄우고 다시 LocalRef의 카드 패널을 킴
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_SearchMissed(PlayerRef localRef, PlayerRef targetRef)
//     {
//         var hand = Server.Instance.spawnedPlayers[targetRef].GetComponent<Player>().GameStat.InGameStat.HandCards;
//
//         bool found = hand.Any(c => c != null && c.Name == "Missed");
//
//         Debug.Log(found);
//         RPC_OpenUseMissedPanel(found, localRef, targetRef);
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void RPC_OpenUseMissedPanel(bool hasMissed, PlayerRef attackPlayerRef, PlayerRef targetPlayerRef)
//     {
//         if (Server.Instance._runner.LocalPlayer != targetPlayerRef) return;
//
//         UIManager.Instance.ShowMissedPanel(hasMissed, attackPlayerRef, targetPlayerRef);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_BroadcastMissedUsage(bool useMissed, PlayerRef localRef, PlayerRef targetRef)
//     {
//         var player = GetPlayer(targetRef);
//
//         if (useMissed)
//         {
//             Debug.Log($"[게임 로그] {player.BasicStat.nickName}님이 '빗나감(Missed)' 카드를 사용했습니다!");
//         }
//         else
//         {
//             Debug.Log($"[게임 로그] {player.BasicStat.nickName}님이 '빗나감(Missed)' 카드를 사용하지 못하여 데미지를 받았습니다!");
//         }
//
//         if (Server.Instance._runner.LocalPlayer != localRef) return;
//
//         UIManager.Instance.cardListPanel.SetActive(true);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_DrawCard(PlayerRef playerRef, int addCount)
//     {
//         List<int> nullIndexes = new List<int>();
//
//         var playerComponent = Server.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
//         var handCards = playerComponent.GameStat.InGameStat.HandCards;
//         var handCardsId = playerComponent.GameStat.InGameStat.HandCardsId;
//
//         for (int i = 0; i < handCards.Length; i++)
//         {
//             if (handCards[i] == null)
//             {
//                 nullIndexes.Add(i);
//             }
//         }
//
//         if (nullIndexes.Count > 0)
//         {
//             for (int i = 0; i < addCount; i++)
//             {
//                 handCards[nullIndexes[i]] = CardSystem.Instance.initDeck[0];
//                 handCardsId[nullIndexes[i]] = CardSystem.Instance.initDeck[0].CardID;
//                 RPC_OnCardButton(playerRef, nullIndexes[i]);
//                 CardSystem.Instance.initDeck.RemoveAt(0);
//             }
//
//             Debug.Log($"{playerRef.ToString()} 덱에 카드 {addCount}장 추가");
//             
//             RPC_ReceiveToHandCardsData(handCardsId);
//         }
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void RPC_OnCardButton(PlayerRef playerRef, int index)
//     {
//         if (Server.Instance._runner.LocalPlayer != playerRef) return;
//
//         UseCardUI.Instance.cardButtons[index].SetActive(true);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_TargetSelectedCard(PlayerRef attacker, PlayerRef target, int cardIndex)
//     {
//         Debug.Log($"attacker:: {attacker}");
//         Debug.Log($"target:: {target}");
//         Debug.Log($"cardId:: {cardIndex}");
//
//         if (Runner.LocalPlayer != attacker) return;
//         var attackPlayer = GetPlayer(attacker);
//         var targetPlayer = GetPlayer(target);
//
//         Debug.Log($"targetPlayer:: {targetPlayer.GameStat.InGameStat.HandCards[cardIndex]}");
//
//         var selectedCard = targetPlayer.GameStat.InGameStat.HandCards[cardIndex];
//
//         Debug.Log($"selectedCard:: {selectedCard}");
//
//         if (targetPlayer != null)
//         {
//             Debug.Log($"{attackPlayer.BasicStat.nickName}님의 공격이 끝났습니다.");
//
//             Debug.Log($"{targetPlayer.BasicStat.nickName}님이 {selectedCard.Name}, {selectedCard.CardID} 카드를 선택함");
//         }
//         else
//         {
//             Debug.LogWarning("Target Player를 찾을 수 없습니다.");
//         }
//
//         TurnManager.Instance.ContinueTurn(attacker);
//     }
//
//     public Player GetPlayer(PlayerRef playerRef)
//     {
//         for (int i = 0; i < syncedPlayerClass.Length; i++)
//         {
//             if (syncedPlayerRefs[i] == playerRef)
//                 return syncedPlayerClass[i];
//         }
//
//         return null;
//     }
//
//     // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     // public void RPC_SetLocalPlayerRef(PlayerRef refToSet)
//     // {
//     //     LocalRef = refToSet;
//     //     Debug.Log($"[서버] Broadcaster에 LocalRef 저장: {refToSet}");
//     // }
//
//     [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
//     public void RPC_ShowResult(string result, string[] playerInfos)
//     {
//         var victoryCheck = FindObjectOfType<VictoryCheck>();
//
//         victoryCheck.gameResult = result;
//         victoryCheck.OpenGameResultUI(playerInfos);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_RequestUseCardList(PlayerRef playerRef, int cardIdx)
//     {
//         Debug.Log($"{playerRef} 클라이언트 → 카드 사용 요청");
//         Debug.Log($"전달된 카드 Number: {cardIdx}");
//
//         var playerComponent = Server.Instance.spawnedPlayers[playerRef].GetComponent<Player>();
//         var cards = playerComponent.GameStat.InGameStat.HandCards;
//
//         if (cards[cardIdx].IsTargetRequired)
//         {
//             RPC_ShowPlayerSelectPanel(playerRef);
//             cards[cardIdx].UseCard(cardIdx);
//             cards[cardIdx] = null;
//         }
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_ShowPlayerSelectPanel(PlayerRef playerRef)
//     {
//         if (Server.Instance._runner.LocalPlayer != playerRef) return;
//
//         UIManager.Instance.ShowPlayerSelectPanel();
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RPC_MakeCombatEvent(PlayerRef senderRef, PlayerRef targetRef, int damage, RpcInfo info = default)
//     {
//         Debug.Log($"호출 주체 {senderRef}");
//
//         var target = InGameSystem.Instance.GetPlayerOrNull(targetRef);
//
//         if (target != null)
//         {
//             CombatEvent combatEvent = new CombatEvent
//             {
//                 Sender = Server.Instance.spawnedPlayers[senderRef].GetComponent<Player>().GameStat.InGameStat,
//                 Receiver = target,
//                 Damage = damage
//             };
//
//             InGameSystem.Instance.AddInGameEvent(combatEvent);
//
//             //RPC로 체력 변경 사항 보내주기
//         }
//         else
//         {
//             Debug.LogWarning("타겟이 없습니다.");
//         }
//     }
//     
//     
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     public void RPC_ReceiveToHandCardsData(int[] handCardIds, RpcInfo info = default)
//     {
//         if (Runner.LocalPlayer != Object.InputAuthority) return;
//
//         Debug.Log($"[Client] 카드 수신: {string.Join(",", handCardIds)}");
//
//         CardUIManager.Instance.UpdateHandCardUI(handCardIds);
//     }
//     
//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_StartPlayerTurn(PlayerRef playerRef)
//     {
//         if (Server.Instance._runner.IsServer)
//         {
//             UIManager.Instance.waitingPanel.SetActive(true);
//         }
//         else if (Server.Instance._runner.LocalPlayer == playerRef)
//         {
//             UIManager.Instance.cardListPanel.SetActive(true);
//         }
//         else
//         {
//             UIManager.Instance.waitingPanel.SetActive(true);
//         }
//         
//         
//     }
//     
//     // [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
//     // public void RPC_RequestUseCardList(PlayerRef playerRef, int cardIndices)
//     // {
//     //     Debug.Log($"{playerRef} 클라이언트 → 카드 사용 요청");
//     //     Debug.Log($"전달된 카드 Number: {cardIndices}");
//     //     
//     //     // 내 플레이어가 아니면 무시
//     //     if (Runner.LocalPlayer != playerRef) return; 
//     //
//     //     var player = Broadcaster.Instance.GetPlayer(playerRef);
//     //     var card = player.GameStat.InGameStat.HandCards[cardIndices];
//     //     
//     //     Debug.Log($"card instance: {card},  name: {card.Name} ,type: {card.GetType()}");
//     //     
//     //     card.UseCard(() => {
//     //         Debug.Log("카드 효과 완료 → 다음 카드 선택 패널 표시");
//     //         
//     //         
//     //         // 지연실행 ( 다시 카드 선택 )
//     //         // Runner.Invoke(() => { 
//     //         //     UIManager.Instance.cardListPanel.SetActive(true);
//     //         // }, delay: 0.5f); // 약간의 딜레이를 주는 것이 부드러움
//     //     });
//     // }
//     
//     [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
//     public void RPC_RequestFinishTurn(PlayerRef playerRef)
//     {
//         Debug.Log($"{playerRef} 턴 종료");
//         Broadcaster.Instance.RPC_ResetPanel();
//         
//         PlayerRef nextPlayer = TurnManager.Instance.EndTurn();
//
//         Debug.Log($"{nextPlayer}");
//         Debug.Log($"턴 변경");
//
//         RPC_StartPlayerTurn(nextPlayer);
//     }
}