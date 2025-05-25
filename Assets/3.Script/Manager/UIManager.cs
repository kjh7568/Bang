using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject cardListPanel;
    public GameObject waitingPanel;
    public GameObject playerChoicePanel;
    public GameObject missedPanel;
    
    public TMP_Text waitingUserTurnText;
    
    [SerializeField] private Button useMissedButton;
    
    //public List<GameObject> enemyList = new List<GameObject>();
    
    // 선택 완료 시 실행할 콜백
    //private Action<string> onTargetSelected; 

    private bool isPlayerSelectActive = false;
    public bool isPanelOn = false;

    [SerializeField] private Button targetButtonPrefab; 
    [SerializeField] private Transform buttonParent;

    private PlayerRef localPlayer;
    private PlayerRef returnPlayer;

    private void Awake()
    {
        Instance = this;
        playerChoicePanel.SetActive(false);

        localPlayer = BasicSpawner.Instance._runner.LocalPlayer;
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
    
    // List<GameObject> allPlayers;
    // GameObject currentPlayer;
    
    public void SetTargetSelectionUI()
    {
        List<PlayerRef> targets = new List<PlayerRef>();
        
        for (int i = 0; i < Broadcaster.Instance.syncedPlayerRefs.Length ; i++)
        {
            var player = Broadcaster.Instance.syncedPlayerRefs[i];
            if (player == localPlayer)
                continue;
    
            targets.Add(player);
        }

        foreach (PlayerRef target in targets)
        {
            Button btn = Instantiate(targetButtonPrefab, buttonParent);
            //btn.GetComponentInChildren<TextMeshProUGUI>().text = target.name;
        
            btn.onClick.AddListener(() =>
            {
                SelectTarget(target);
            });
        }
    }

    void SelectTarget(PlayerRef target)
    {
        playerChoicePanel.SetActive(false);

        Broadcaster.Instance.RPC_AttackPlayerNotify(localPlayer, target);
    }

    public void ShowPlayerSelectPanel(Action<string> onTargetSelectedCallback)
    {
        playerChoicePanel.SetActive(true);
    }

    public void ShowMissedPanel(bool hasMissed, PlayerRef playerRef)
    {
        waitingPanel.SetActive(false);
        missedPanel.SetActive(true);

        useMissedButton.enabled = hasMissed;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible    = true;

        returnPlayer = playerRef;
    }

    public void DontUseMissed()
    {
        Broadcaster.Instance.RPC_MakeCombatEvent(BasicSpawner.Instance._runner.LocalPlayer, returnPlayer, 1);
    }
    
    
    private Action<int> _onCardSelectedCallback;
    
    public void ShowCardSelectionPanel(Action<int> onCardSelectedID)
    {
        cardListPanel.SetActive(true);
        waitingPanel.SetActive(false);

        _onCardSelectedCallback = onCardSelectedID;
    }
    
    public void OnCardSelected(int index)
    {
        cardListPanel.SetActive(false);
        
        _onCardSelectedCallback?.Invoke(index);
        _onCardSelectedCallback = null;
    }
    
    public void ShowWaitingForTargetPanel()
    {
        cardListPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }
    
    public void ShowCardTargetPanel()
    {
        cardListPanel.SetActive(true);
        waitingPanel.SetActive(false);
    }
}
