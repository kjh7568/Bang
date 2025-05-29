using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HumanList humanList;
    public JobList jobList;

    public GameObject loadingUI;
    [SerializeField] private Slider loadingBar;

    public bool isDead = false;
    
    public Transform[] spawnPoints;

    private Player turnOwner;

    private void Awake()
    {
        Instance = this;
        
        // 로딩 UI 활성화
        loadingUI.SetActive(true);
        StartLoading();
    }

    private void Start()
    {
        // 플레이어 카메라 설정 ( ALL )
        CameraManager.Instance.GetCameraByIndex(Player.LocalPlayer.BasicStat.iD);
        
        //클라에서는 안도는게 맞음
        if(!Server.Instance._runner.IsServer) return;

        InitializeGame();
    }

    private void Update()
    {
        if (Player.LocalPlayer.InGameStat.hp <= 0 && !isDead)
        {
            isDead = true;
            Broadcaster.Instance.RPC_VictoryCheck(Player.LocalPlayer.playerRef);

            if (isDead)
            {
                UIManager.Instance.ResetPanel();
                UIManager.Instance.deadPanel.SetActive(true);
                PlayerDead();
            }
        }
    }

    public IEnumerator InitializeGame()
    {
        yield return null;

        SetPlayerHuman();
        SetPlayerJob();
        
        Server.Instance.MovePlayersToSpawnPoints(spawnPoints);
        
        turnOwner = GetFirstTurnPlayer();
        
        CardSystem.Instance.Init();

        Broadcaster.Instance.RPC_PlayerHpSync();
        Broadcaster.Instance.RPC_EndLoading();
        Broadcaster.Instance.RPC_StartPlayerTurn(turnOwner.playerRef);
        Broadcaster.Instance.RPC_SetClientPanel();
        
        SoundManager.Instance.PlayInBackground();
    }
    
    public void StartLoading()
    {
        loadingBar.value = 0;
        loadingBar.DOValue(1f, 3f).SetEase(Ease.InOutQuad);
    }

    public void EndLoading()
    {
        loadingUI.SetActive(false);
    }

    private Player GetFirstTurnPlayer()
    {
        for (int i = 0; i < Player.ConnectedPlayers.Count; i++)
        {
            if (Player.ConnectedPlayers[i].InGameStat.MyJob.Name == "보안관")
            {
                Broadcaster.Instance.turnIdx = i + 1;
                Player.ConnectedPlayers[i].SyncPlayerHp++;
                Player.ConnectedPlayers[i].InGameStat.hp++;
                
                return Player.GetPlayer(i + 1);
            }
        }
        
        //나중에 지울 것
        return Player.GetPlayer(Broadcaster.Instance.turnIdx);
    }

    private void SetPlayerHuman()
    {
        var randomHumanList = Enumerable.Range(0, humanList.humanList.Count).OrderBy(_ => Random.value).ToList();

        for (int i = 1; i <= Player.ConnectedPlayers.Count; i++)
        {
            Broadcaster.Instance.RPC_SendPlayerHuman(Player.GetPlayer(i).playerRef, randomHumanList[i - 1]);
        }
    }

    private void SetPlayerJob()
    {
        var randomJobList = Enumerable.Range(0, jobList.jobList.Count).OrderBy(_ => Random.value).ToList();

        for (int i = 1; i <= Player.ConnectedPlayers.Count; i++)
        {
            Broadcaster.Instance.RPC_SendPlayerJob(Player.GetPlayer(i).playerRef, randomJobList[i - 1]);
        }
    }

    private void PlayerDead()
    {
        Broadcaster.Instance.RPC_PlayerDead(Player.LocalPlayer);
    }
}
