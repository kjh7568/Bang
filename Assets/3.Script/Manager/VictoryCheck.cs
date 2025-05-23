using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class VictoryCheck : MonoBehaviour
{
    [SerializeField] private GameObject gameResultUI;
    
    [SerializeField] private TMP_Text gameResultText;
    [SerializeField] private TMP_Text[] playerTexts;
    
    private string gameResult;
    
    public void CheckVictoryConditions()
    {
        string[] playerInfos = new string[4]; // 플레이어 순서 보안관/무법자/무법자/배신자
        int outlawIndex = 1;

        foreach (var player in GameManager.Instance.players)
        {
            string nickname = player.BasicStat.nickName;
            string human = player.GameStat.InGameStat.MyHuman.Name;
            string job = player.GameStat.InGameStat.MyJob.Name;
            string info = $"{nickname}\n{human}\n{job}";

            if (job == "보안관") playerInfos[0] = info;
            else if (job == "무법자" && outlawIndex <= 2) playerInfos[outlawIndex++] = info;
            else if (job == "배신자") playerInfos[3] = info;
        }
        
        string result = GetGameResult();
        if (string.IsNullOrEmpty(result)) return;
        OpenGameResultUI(playerInfos);

        
        // RPC_ShowResult(result, playerInfos);
    }
    private string GetGameResult()
    {
        List<Player> players = GameManager.Instance.players;
        Player sheriff = null;
        List<Player> outlaws = new();
        Player renegade = null;

        foreach (var player in players)
        {
            var jobName = player.GameStat.InGameStat.MyJob.Name;
            if (jobName == "보안관") sheriff = player;
            else if (jobName == "무법자") outlaws.Add(player);
            else if (jobName == "배신자") renegade = player;
        }

        if (sheriff != null && !sheriff.GameStat.InGameStat.IsDead)
        {
            bool allEnemiesDead = outlaws.TrueForAll(o => o.GameStat.InGameStat.IsDead);
            if (renegade != null && !renegade.GameStat.InGameStat.IsDead)
                allEnemiesDead = false;

            if (allEnemiesDead)
                return "sheriff is win!";
        }

        if (sheriff != null && sheriff.GameStat.InGameStat.IsDead)
        {
            if (outlaws.Exists(o => !o.GameStat.InGameStat.IsDead))
                return "outlaw is win!";
        }

        int aliveCount = 0;
        Player lastAlive = null;
        foreach (var p in players)
        {
            if (!p.GameStat.InGameStat.IsDead)
            {
                aliveCount++;
                lastAlive = p;
            }
        }

        if (aliveCount == 1 && lastAlive == renegade)
            return "renegade is win!";

        return ""; // 아직 승리 조건이 안 됨
    }

// Rpc는 간단히 결과만 전달
    

    private void OpenGameResultUI(string[] playerInfos)
    {
        gameResultUI.SetActive(true);
        gameResultText.text = gameResult;

        for (int i = 0; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerInfos[i]; // 사전에 넘겨 받은 텍스트로 설정
        }
    }
}
