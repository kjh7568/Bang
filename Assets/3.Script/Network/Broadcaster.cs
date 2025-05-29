using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
public class Broadcaster : NetworkBehaviour
{
    public static Broadcaster Instance;

    public int turnIdx = 1;

    public List<int> deadPlayers = new();
    private HashSet<PlayerRef> clientsReady = new();

    private int missedCheckCount;
    
    public override void Spawned()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        missedCheckCount = 0;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartPlayerTurn(PlayerRef playerRef)
    {
        //애니메이션
        Player player = Player.GetPlayer(playerRef);
        
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetTrigger("drawing");
        Server.Instance.MovePlayersToSpawnPoints(GameManager.Instance.spawnPoints);
        //애니메이션
        
        if (Runner.IsServer)
        {
            DrawCard(playerRef);
            RPC_ReceiveHandCardAndUpdateUi(playerRef, Player.GetPlayer(playerRef).InGameStat.HandCardsId);
        }

        if (Runner.LocalPlayer == playerRef)
        {
            player.InGameStat.isBang = false;
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.cardListPanel.SetActive(true);
        }
        else if (GameManager.Instance.isDead == false)
        {
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }

    private void DrawCard(PlayerRef playerRef)
    {
        if (CardSystem.Instance.initDeck.Count == 0)
        {
            CardSystem.Instance.initDeck = CardSystem.Instance.UsedDeck.OrderBy(x => Random.value).ToList();
            CardSystem.Instance.UsedDeck.Clear();
        }
        
        int drawCardId = CardSystem.Instance.initDeck[0].CardID;

        var handCardID = Player.GetPlayer(playerRef).InGameStat.HandCardsId;

        for (int i = 0; i < handCardID.Length; i++)
        {
            if (handCardID[i] == 0)
            {
                Player.GetPlayer(playerRef).InGameStat.HandCardsId[i] = drawCardId;

                CardSystem.Instance.initDeck.RemoveAt(0);
                break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestDrawCard(PlayerRef playerRef)
    {
        DrawCard(playerRef);
        RPC_ReceiveHandCardAndUpdateUi(playerRef, Player.GetPlayer(playerRef).InGameStat.HandCardsId);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestEndTurn()
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        turnIdx = turnIdx % Runner.ActivePlayers.Count() + 1;

        for (int i = 0; i < deadPlayers.Count; i++)
        {
            if (deadPlayers.Contains(turnIdx))
            {
                turnIdx = turnIdx % Runner.ActivePlayers.Count() + 1;
            }
        }
        
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

    public void RequestUseCard(PlayerRef playerRef, int cardIdx)
    {
        if (Runner.IsServer && Runner.LocalPlayer == playerRef)
        {
            // 서버가 자기 자신이면 RPC 대신 직접 처리
            HandleUseCard(playerRef, cardIdx);
        }
        else
        {
            // 그 외의 경우는 RPC로 요청
            RPC_RequestUseCard(playerRef, cardIdx);
        }
    }
    
    public void HandleUseCard(PlayerRef playerRef, int cardIdx)
    {
        int cardId = Player.GetPlayer(playerRef).InGameStat.HandCardsId[cardIdx];
        var usedCard = CardSystem.Instance.GetCardByIDOrNull(cardId);

        if (usedCard != null)
        {
            CardSystem.Instance.UsedDeck.Add(usedCard);
        }

        Player.GetPlayer(playerRef).InGameStat.HandCardsId[cardIdx] = 0;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUseCard(PlayerRef playerRef, int cardIdx)
    {
        HandleUseCard(playerRef, cardIdx);
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

            if (GameManager.Instance.jobList.jobList[jobIdx].Name.Equals("보안관"))
            {
                Player.LocalPlayer.SyncPlayerHp = 5;
                Player.LocalPlayer.InGameStat.hp = 5;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_EndLoading(RpcInfo info = default)
    {
        GameManager.Instance.EndLoading();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestBangAim(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);

        Vector3 direction = target.transform.position - attacker.transform.position;
        
        direction.y = 0f;
        Debug.Log("direction: " + direction);
        if (direction != Vector3.zero)
        {
            Debug.Log("방향전환중");
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            attacker.transform.rotation = lookRotation;
            
            // 회전을 fixedUpdate에서 적용
            // 너 내가 여기로 회전하고 싶어 예약
        }
        
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RequestBang(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);

        Animator attackerAnimator = attacker.GetComponent<Animator>();
        attackerAnimator.SetTrigger("pointing");
        
        if (Runner.LocalPlayer == targetRef)
        {
            var hasMissed = CardSystem.Instance.CheckHasMissed(targetRef);
            
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.ShowMissedPanel(hasMissed, attackRef, targetRef);
        }
        else if (GameManager.Instance.isDead == false)
        {
            UIManager.Instance.waitingPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RequestGatling(PlayerRef attackRef)
    {
        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.waitingPanel.SetActive(true);
        }
        else
        {
            var targetRef = Player.LocalPlayer.playerRef;
            var hasMissed = CardSystem.Instance.CheckHasMissed(targetRef);
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.ShowMissedPanel_Gatling(hasMissed, attackRef, targetRef);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyMissed(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);
        
        Animator attackerAnimator = attacker.GetComponent<Animator>();
        Animator targetAnimator =target.GetComponent<Animator>();
        Debug.Log("Missed : "+ attacker.name + " : " + target.name + " : " );
        
        Vector3 direction = target.transform.position - attacker.transform.position;
        
        direction.y = 0f;
        Debug.Log("direction: " + direction);
        if (direction != Vector3.zero)
        {
            Debug.Log("방향전환중");
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            attacker.transform.rotation = lookRotation;
            
            // 회전을 fixedUpdate에서 적용
            // 너 내가 여기로 회전하고 싶어 예약
        }
        
        attackerAnimator.SetTrigger("shooting");
        SoundManager.Instance.PlaySound(SoundType.Bang);

        targetAnimator.SetTrigger("dodging");
        SoundManager.Instance.PlaySound(SoundType.Dodging);

        Server.Instance.MovePlayersToSpawnPoints(GameManager.Instance.spawnPoints);
        //애니메이션
        
        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.cardListPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyMissed_Gatling(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);
        
        Animator attackerAnimator = attacker.GetComponent<Animator>();
        Animator targetAnimator =target.GetComponent<Animator>();
        Debug.Log("Missed : "+ attacker.name + " : " + target.name + " : " );
        
        Vector3 direction = target.transform.position - attacker.transform.position;
        
        direction.y = 0f;
        Debug.Log("direction: " + direction);
        if (direction != Vector3.zero)
        {
            Debug.Log("방향전환중");
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            attacker.transform.rotation = lookRotation;
            
            // 회전을 fixedUpdate에서 적용
            // 너 내가 여기로 회전하고 싶어 예약
        }
        
        attackerAnimator.SetTrigger("shooting");
        SoundManager.Instance.PlaySound(SoundType.Bang);

        targetAnimator.SetTrigger("dodging");
        SoundManager.Instance.PlaySound(SoundType.Dodging);

        Server.Instance.MovePlayersToSpawnPoints(GameManager.Instance.spawnPoints);
        //애니메이션

        if (Runner.LocalPlayer == targetRef)
        {
            RPC_SetCountAndGatling(attackRef);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyBang(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);
        
        Animator attackerAnimator = attacker.GetComponent<Animator>();
        Animator targetAnimator =target.GetComponent<Animator>();
        Debug.Log("Bang : "+ attacker.name + " : " + target.name + " : " );
        attackerAnimator.SetTrigger("shooting");
        targetAnimator.SetTrigger("hitting");
        
        SoundManager.Instance.PlaySound(SoundType.Bang);

        //애니메이션
        
        if (Runner.IsServer && Runner.LocalPlayer != targetRef)
        {
            Player.GetPlayer(targetRef).InGameStat.hp--;
        }

        if (Runner.IsServer)
        {
            Player.GetPlayer(targetRef).SyncPlayerHp--;
            RPC_PlayerHpSync();
        }

        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.cardListPanel.SetActive(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyGatling(PlayerRef attackRef, PlayerRef targetRef)
    {
        //애니메이션
        Player attacker = Player.GetPlayer(attackRef);
        Player target = Player.GetPlayer(targetRef);
        
        Animator attackerAnimator = attacker.GetComponent<Animator>();
        Animator targetAnimator =target.GetComponent<Animator>();
        Debug.Log("Gatling : "+ attacker.name + " : " + target.name + " : " );
        attackerAnimator.SetTrigger("shooting");
        targetAnimator.SetTrigger("hitting");
        
        SoundManager.Instance.PlaySound(SoundType.Bang);

        //애니메이션
        
        if (Runner.IsServer && Runner.LocalPlayer != targetRef)
        {
            Player.GetPlayer(targetRef).InGameStat.hp--;
        }

        if (Runner.IsServer)
        {
            Player.GetPlayer(targetRef).SyncPlayerHp--;
            RPC_PlayerHpSync();
        }

        if (Runner.LocalPlayer == targetRef)
        {
            RPC_SetCountAndGatling(attackRef);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetCountAndGatling(PlayerRef attackRef)
    {
        Debug.Log("카운트 올라유~");
        
        missedCheckCount++;
        
        if (missedCheckCount > 2)
        {
            Debug.Log("카운트 다 올랐어유~");
            
            RPC_TurnOnPanel(attackRef);
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TurnOnPanel(PlayerRef attackRef)
    {
        if (Runner.LocalPlayer == attackRef)
        {
            UIManager.Instance.waitingPanel.SetActive(false);
            UIManager.Instance.cardListPanel.SetActive(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyBeer(PlayerRef playerRef)
    {
        //애니메이션
        Player player = Player.GetPlayer(playerRef);
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetTrigger("drinking");
        
        SoundManager.Instance.PlaySound(SoundType.Beer);
        
        Server.Instance.MovePlayersToSpawnPoints(GameManager.Instance.spawnPoints);
        //애니메이션
        if (Runner.IsServer && Runner.LocalPlayer != playerRef)
        {
            Player.GetPlayer(playerRef).InGameStat.hp ++;
        }

        if (Runner.IsServer)
        {
            Player.GetPlayer(playerRef).SyncPlayerHp ++;
            RPC_PlayerHpSync();
        }
    }
 
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayerHpSync()
    {
        foreach (var player in Player.ConnectedPlayers)
        {
            var ui = player.GetComponentInChildren<PlayerUI>();
            if (ui != null)
                ui.UpdatePlayerHp();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_VictoryCheck(PlayerRef playerRef)
    {
        UIManager.Instance.ResetPanel();
        //애니메이션
        Player player = Player.GetPlayer(playerRef);
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetTrigger("dying");
        Server.Instance.MovePlayersToSpawnPoints(GameManager.Instance.spawnPoints);
        //애니메이션
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

        // 승리 조건 판단
        if (!sheriffAlive)
        {
            if (outlawAlive > 0)
            {
                result = "무법자 승리!";
            }
            else if (renegadeAlive)
            {
                result = "배신자 승리!";
            }
        }
        else if (outlawAlive == 0 && !renegadeAlive)
        {
            result = "보안관 승리!";
        }

        // 승리가 결정되었다면 이름 정리 후 결과 RPC 전송
        if (result != "not victory yet")
        {
            string[] playerNames = new string[4]; // 보안관, 무법자1, 무법자2, 배신자
            int outlawIndex = 1;

            foreach (var p in players)
            {
                if (p.InGameStat == null || p.InGameStat.MyJob == null) continue;

                string name = p.BasicStat.nickName;
                string job = p.InGameStat.MyJob.Name;

                if (job == "보안관") playerNames[0] = name;
                else if (job == "무법자" && outlawIndex <= 2) playerNames[outlawIndex++] = name;
                else if (job == "배신자") playerNames[3] = name;
            }

            RPC_ShowResultToClients(result, playerNames);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowResultToClients(string result, string[] playerNames)
    {
        UIManager.Instance.ShowResultPanel(result);

        // 모든 클라이언트가 받은 데이터로 이름 설정
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (UIManager.Instance.resultPlayerNameText[i] != null)
                UIManager.Instance.resultPlayerNameText[i].text = playerNames[i];
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetClientPanel()
    {
        FindObjectOfType<MyInfoPanel>().InitializedMyInfoPanel();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendMyNickName2Server(string nickname, PlayerRef playerRef)
    {
        Server.Instance.nicknameBuffer.Add(nickname);

        Player.GetPlayer(playerRef).BasicStat.nickName = nickname;

        RPC_SetNickNameUi(Server.Instance.nicknameBuffer.ToArray());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetNickNameUi(string[] nicknames)
    {
        Debug.Log("UI 수정 완료!");
        FindObjectOfType<WatingSetting>()?.UpdateNicknameTexts(nicknames);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateNicknames(string[] nicknames)
    {
        WatingSetting ui = FindObjectOfType<WatingSetting>();
        if (ui != null)
            ui.UpdateNicknameTexts(nicknames);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ClientReady(PlayerRef playerRef)
    {
        clientsReady.Add(playerRef);

        if (clientsReady.Count == Runner.ActivePlayers.Count())
        {
            StartCoroutine(GameManager.Instance.InitializeGame());
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerDead(Player player)
    {
        Player.RemovePlayer(player);
        deadPlayers.Add(player.playerRef.AsIndex);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestAllNicknames(PlayerRef requester)
    {
        List<PlayerRef> refs = new();
        List<string> nicks = new();

        foreach (var p in Player.ConnectedPlayers)
        {
            refs.Add(p.playerRef);
            nicks.Add(p.BasicStat.nickName);
        }

        RPC_ReceiveAllNicknames(requester, refs.ToArray(), nicks.ToArray());
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveAllNicknames(PlayerRef requester, PlayerRef[] refs, string[] nicks)
    {
        if (Runner.LocalPlayer != requester) return;

        for (int i = 0; i < refs.Length; i++)
        {
            UIManager.NicknameCache[refs[i]] = nicks[i];
        }
    }
}