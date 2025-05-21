using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryCheck : MonoBehaviour
{
    [SerializeField] private TMP_Text gameResultText;
    
    private string gameResult;
    
    [SerializeField] private GameObject gameResultUI;
    
    [SerializeField] private TMP_Text[] playerTexts;
    public void CheckVictoryConditions()
    {
        List<Player> players = GameManager.Instance.players;

        Player sheriff = null;
        List<Player> outlaws = new();
        Player renegade = null;
        

        foreach (var player in players)
        {
            var jobName = player.GameStat.InGameStat.MyJob.Name;
            if (jobName == "보안관")
            {
                sheriff = player;
            }
            else if (jobName == "무법자")
            {
                outlaws.Add(player);
            }
            else if (jobName == "배신자")
            {
                renegade = player;
            }
        }

        // 보안관 승리 조건: 자신 살아있고 무법자 + 배신자 모두 죽어야 함
        if (sheriff != null && !sheriff.GameStat.InGameStat.IsDead)
        {
            bool allEnemiesDead = true;
            foreach (var outlaw in outlaws)
            {
                if (!outlaw.GameStat.InGameStat.IsDead)
                {
                    allEnemiesDead = false;
                    break;
                }
            }

            if (renegade != null && !renegade.GameStat.InGameStat.IsDead)
            {
                allEnemiesDead = false;
            }

            if (allEnemiesDead)
            {
                Debug.Log("보안관 승리!");
                gameResult = "sheriff is win!";
                OpenGameResultUI();
                return;
            }
        }

        // 무법자 승리 조건: 보안관이 죽었을 경우
        if (sheriff != null && sheriff.GameStat.InGameStat.IsDead)
        {
            foreach (var outlaw in outlaws)
            {
                if (!outlaw.GameStat.InGameStat.IsDead)
                {
                    Debug.Log("무법자 승리!");
                    gameResult = "outlaw is win!";
                    OpenGameResultUI();
                    return;
                }
            }
        }

        // 배신자 승리 조건: 혼자 살아남은 경우
        int aliveCount = 0;
        Player lastAlive = null;

        foreach (var player in players)
        {
            if (!player.GameStat.InGameStat.IsDead)
            {
                aliveCount++;
                lastAlive = player;
            }
        }

        if (aliveCount == 1 && lastAlive == renegade)
        {
            Debug.Log("배신자 승리!");
            gameResult = "renegade is win!";
            OpenGameResultUI();
        }

        
    }

    private void OpenGameResultUI()
    {
        gameResultUI.SetActive(true);
        gameResultText.text = gameResult;
        
        int outlawIndex = 1; // 무법자용 인덱스 (1번, 2번)
    
        foreach (var player in GameManager.Instance.players)
        {
            string nickname = player.BasicStat.nickName;
            string human = player.GameStat.InGameStat.MyHuman.Name;
            string job = player.GameStat.InGameStat.MyJob.Name;

            string info = $"{nickname}\n{human}\n{job}";

            if (job == "보안관")
            {
                playerTexts[0].text = info;
            }
            else if (job == "무법자" && outlawIndex <= 2)
            {
                playerTexts[outlawIndex].text = info;
                outlawIndex++;
            }
            else if (job == "배신자")
            {
                playerTexts[3].text = info;
            }
        }
        
    }
}
