using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject cardListPanel;
    public GameObject waitingPanel;
    public GameObject playerPanel;
    
    public TMP_Text waitingUserTurnText;
    //public List<GameObject> enemyList = new List<GameObject>();
    
    // 선택 완료 시 실행할 콜백
    //private Action<string> onTargetSelected; 

    private bool isPlayerSelectActive = false;
    public bool isPanelOn = false;
    
    private void Awake()
    {
        Instance = this;
        //InitializePlayerPanelButtons();
        playerPanel.SetActive(false); 
    }
    
    private void Update()
    {
        if (cardListPanel.activeInHierarchy)
        {
            isPanelOn = true;
        }
        else
        {
            isPanelOn = false;
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cardListPanel.activeInHierarchy)
            {
                cardListPanel.SetActive(false);
                isPanelOn = false;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible    = false;
            }
            else
            {
                cardListPanel.SetActive(true);
                isPanelOn = true;
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible    = true;
            }
        }
    }
    
    [SerializeField] Button targetButtonPrefab; // 버튼 프리팹
    [SerializeField] Transform buttonParent; // 버튼들 넣을 위치 (Vertical Layout Group 사용 추천)

    // List<GameObject> allPlayers;
    // GameObject currentPlayer;
    
    public Player localPlayer;

    public void SetTargetSelectionUI()
    {
        List<Player> targets = new List<Player>();
        
        for (int i = 0; i < Broadcaster.Instance.syncedPlayerClass.Length ; i++)
        {
            var player = Broadcaster.Instance.syncedPlayerClass[i];
            if (player == localPlayer)
                continue;
    
            targets.Add(player);
        }

        foreach (Player target in targets)
        {
            Button btn = Instantiate(targetButtonPrefab, buttonParent);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = target.name;
        
            btn.onClick.AddListener(() =>
            {
                SelectTarget(target);
            });
        }
    }

    void SelectTarget(Player target)
    {
        Debug.Log($"{target.name}을(를) 공격 대상으로 선택함!");
        playerPanel.SetActive(false);

        // 공격 로직 실행
    }

    // private void InitializePlayerPanelButtons()
    // {
    //     int index = 0;
    //
    //     foreach (var playerPair in BasicSpawner.Instance.spawnedPlayers)
    //     {
    //         var playerObj = playerPair.Value;
    //         var player = playerObj.GetComponent<Player>();
    //         
    //         if (player.Object.InputAuthority == BasicSpawner.Instance._runner.LocalPlayer)
    //             continue;
    //
    //         if (index < enemyList.Count)
    //         {
    //             GameObject slot = enemyList[index];
    //
    //             Player enemySlot = slot.GetComponent<Player>();
    //             enemySlot = player;
    //
    //             TMP_Text nameText = slot.GetComponentInChildren<TMP_Text>();
    //             if (nameText != null)
    //                 nameText.text = player.BasicStat.nickName;
    //
    //             index++;
    //         }
    //     }
    // }
    
    // private void InitializePlayerPanelButtons()
    // {
    //     int index = 0;
    //
    //     foreach (var playerPair in BasicSpawner.Instance.spawnedPlayers)
    //     {
    //         var player = playerPair.Value.GetComponent<Player>();
    //
    //         if (player.Object.InputAuthority != BasicSpawner.Instance._runner.LocalPlayer)
    //         {
    //             if (index < enemyList.Count)
    //             {
    //                 GameObject enemySlot = enemyList[index];
    //
    //                 TMP_Text nameText = enemySlot.GetComponentInChildren<TMP_Text>();
    //                 if (nameText != null)
    //                     nameText.text = player.BasicStat.nickName;
    //
    //                 Button button = enemySlot.GetComponent<Button>();
    //                 if (button != null)
    //                 {
    //                     string playerName = player.BasicStat.nickName; // 클로저 문제 방지
    //
    //                     button.onClick.RemoveAllListeners();
    //                     button.onClick.AddListener(() => OnEnemySlotClicked(playerName));
    //                 }
    //
    //                 index++;
    //             }
    //         }
    //     }
    // }

    public void ShowPlayerSelectPanel(Action<string> onTargetSelectedCallback)
    {
        playerPanel.SetActive(true);
    }
    
    public void OnEnemySlotClicked(string playerName)
    {
        playerPanel.SetActive(false);
        isPlayerSelectActive = false;
    }
}
