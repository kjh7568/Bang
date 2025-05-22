using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject cardListPanel;
    public GameObject waitingPanel;
    public GameObject playerPanel;
    
    public TMP_Text waitingUserTurnText;
    
    public List<GameObject> enemyList = new List<GameObject>();
    
    // 선택 완료 시 실행할 콜백
    private Action<string> onTargetSelected; 

    private bool isPlayerSelectActive = false;
    
    private void Awake()
    {
        Instance = this;
        InitializePlayerPanelButtons();
        playerPanel.SetActive(false); 
    }

    private void InitializePlayerPanelButtons()
    {
        int index = 0;

        foreach (var playerPair in BasicSpawner.Instance.spawnedPlayers)
        {
            var player = playerPair.Value.GetComponent<Player>();

            if (player.Object.InputAuthority != BasicSpawner.Instance._runner.LocalPlayer)
            {
                if (index < enemyList.Count)
                {
                    GameObject enemySlot = enemyList[index];

                    TMP_Text nameText = enemySlot.GetComponentInChildren<TMP_Text>();
                    if (nameText != null)
                        nameText.text = player.BasicStat.nickName;

                    Button button = enemySlot.GetComponent<Button>();
                    if (button != null)
                    {
                        string playerName = player.BasicStat.nickName; // 클로저 문제 방지

                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => OnEnemySlotClicked(playerName));
                    }

                    index++;
                }
            }
        }
    }

    public void ShowPlayerSelectPanel(Action<string> onTargetSelectedCallback)
    {
        if (isPlayerSelectActive)
        {
            Debug.LogWarning("이미 플레이어 선택 UI가 열려있습니다. 중복 호출 방지.");
            return;
        }
    
        this.onTargetSelected = onTargetSelectedCallback;
        playerPanel.SetActive(true);
        isPlayerSelectActive = true;
    }
    
    public void OnEnemySlotClicked(string playerName)
    {
        playerPanel.SetActive(false);
        isPlayerSelectActive = false;
    
        onTargetSelected?.Invoke(playerName);
        onTargetSelected = null; 
    }
}
