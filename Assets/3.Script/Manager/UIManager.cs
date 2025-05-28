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
    public GameObject[] cardButtons;
    
    public GameObject waitingPanel;
    
    [SerializeField] private Button endTurnButton;
    
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private GameObject targetTextPanel;
    [SerializeField] private Button targetButtonPrefab; 
    
    public GameObject missedPanel;
    [SerializeField] private Button useMissedButton;
    [SerializeField] private Button dontUseMissedButton;
    
    
    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private TMP_Text resultText;
    // public TMP_Text waitingUserTurnText;
    //
    //
    //
    // //public List<GameObject> enemyList = new List<GameObject>();
    //
    // // 선택 완료 시 실행할 콜백
    // //private Action<string> onTargetSelected; 
    //
    // private bool isPlayerSelectActive = false;
    // public bool isPanelOn = false;
    //
    // [SerializeField] private Transform buttonParent;
    //
    // private PlayerRef localPlayer;
    // private PlayerRef returnPlayer;
    //
    private void Awake()
    {
        Instance = this;
        
        endTurnButton.onClick.AddListener(() => {
            // Broadcaster.Instance.RPC_SendMyCardId2Server(Player.LocalPlayer.playerRef, Player.LocalPlayer.InGameStat.HandCardsId);
            Broadcaster.Instance.RPC_RequestEndTurn();
        });        
        // playerChoicePanel.SetActive(false);
        //
        // localPlayer = Server.Instance._runner.LocalPlayer;
    }

    public void ResetPanel()
    {
        cardListPanel.SetActive(false);
        waitingPanel.SetActive(false);
    }
    
    public void OnCardClicked(int index)
    {
        cardListPanel.SetActive(false);
        cardButtons[index].SetActive(false);
        
        PlayerRef playerRef = Player.LocalPlayer.playerRef;
        int cardID = Player.GetPlayer(playerRef).InGameStat.HandCardsId[index];
        var card = CardSystem.Instance.GetCardByIDOrNull(cardID);
        
        Player.LocalPlayer.InGameStat.HandCardsId[index] = 0;
            
        if (card.IsTargetRequired) // 대상 필요 여부
        {
            // 대상 지정 UI 패널 열기
            ShowTargetSelectionPanel((targetRef) =>
            {
                // 대상 선택이 끝났을 때 실행
                CardSystem.Instance.DoActionByName(card.Name, playerRef, targetRef);
            });
        }
        else
        {
            // 바로 실행 가능한 카드 (예: 맥주)
            CardSystem.Instance.DoActionByName(card.Name, playerRef);
            cardListPanel.SetActive(true);
        }

        Player.GetPlayer(playerRef).InGameStat.HandCardsId[index] = 0;
        Broadcaster.Instance.RPC_RequestUseCard(Player.LocalPlayer.playerRef, index);
    }
    
    public void UpdateHandCardUI(int[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == 0)
            {
                cardButtons[i].SetActive(false);
                continue;    
            }

            cardButtons[i].SetActive(true);
            cardButtons[i].GetComponent<Image>().sprite = CardSystem.Instance.GetCardByIDOrNull(cards[i]).CardSprite;
        }
    }
    
    public void ShowTargetSelectionPanel(Action<PlayerRef> onTargetSelected)
    {
        foreach (Transform child in targetTextPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var targetPlayer in Player.ConnectedPlayers)
        {
            if (targetPlayer == Player.LocalPlayer || targetPlayer == null) continue;
            
            var playerRef = targetPlayer.playerRef;
            var button = Instantiate(targetButtonPrefab, targetTextPanel.transform);
            button.GetComponentInChildren<TMP_Text>().text = playerRef.ToString();

            button.onClick.RemoveAllListeners();
            
            button.onClick.AddListener(() =>
            {
                targetPanel.SetActive(false);
                onTargetSelected.Invoke(playerRef);
            });
        }
        
        targetPanel.SetActive(true);
    }
    
    public void ShowMissedPanel(bool hasMissed, PlayerRef attackRef, PlayerRef targetRef)
    {
        Debug.Log($"빗나감 패널 메서드 시작");
        
        useMissedButton.onClick.RemoveAllListeners();
        dontUseMissedButton.onClick.RemoveAllListeners();
        
        useMissedButton.interactable = hasMissed;
    
        missedPanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible    = true;
        
        useMissedButton.onClick.AddListener(() =>
        {
            missedPanel.SetActive(false);
            waitingPanel.SetActive(true);
            
            var cardID = Player.GetPlayer(targetRef).InGameStat.HandCardsId;
         
            for (int i = 0; i < cardID.Length; i++)
            {
                if (cardID[i] == 0) continue;
                
                var card = CardSystem.Instance.GetCardByIDOrNull(cardID[i]);

                if (card.Name == "Missed")
                {
                    Player.GetPlayer(targetRef).InGameStat.HandCardsId[i] = 0;
                    Broadcaster.Instance.RPC_RequestUseCard(Player.LocalPlayer.playerRef, i);
                    Broadcaster.Instance.RPC_NotifyMissed(attackRef, targetRef);
                    return;
                }
            }
        });
        
        dontUseMissedButton.onClick.AddListener(() =>
        {
            missedPanel.SetActive(false);
            waitingPanel.SetActive(true);

            Player.GetPlayer(targetRef).InGameStat.hp--;
            Broadcaster.Instance.RPC_NotifyBang(attackRef, targetRef);
        });
        
    }
    public void ShowResultPanel(string result)
    {
        ResultPanel.SetActive(true);
        resultText.text = result;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible    = true;
    }
    //
    // private void Update()
    // {
    //     if (cardListPanel.activeInHierarchy)
    //     {
    //         isPanelOn = true;
    //     }
    //     else
    //     {
    //         isPanelOn = false;
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         if (cardListPanel.activeInHierarchy)
    //         {
    //             cardListPanel.SetActive(false);
    //             isPanelOn = false;
    //             
    //             Cursor.lockState = CursorLockMode.Locked;
    //             Cursor.visible    = false;
    //         }
    //         else
    //         {
    //             cardListPanel.SetActive(true);
    //             isPanelOn = true;
    //             
    //             Cursor.lockState = CursorLockMode.None;
    //             Cursor.visible    = true;
    //         }
    //     }
    // }
    //
    // // List<GameObject> allPlayers;
    // // GameObject currentPlayer;
    //
    // public void SetTargetSelectionUI()
    // {
    //     List<PlayerRef> targets = new List<PlayerRef>();
    //     
    //     for (int i = 0; i < Broadcaster.Instance.syncedPlayerRefs.Length ; i++)
    //     {
    //         var player = Broadcaster.Instance.syncedPlayerRefs[i];
    //         if (player == localPlayer)
    //             continue;
    //
    //         targets.Add(player);
    //     }
    //
    //     foreach (PlayerRef target in targets)
    //     {
    //         Button btn = Instantiate(targetButtonPrefab, buttonParent);
    //         btn.GetComponentInChildren<TextMeshProUGUI>().text = target.ToString();
    //     
    //         btn.onClick.AddListener(() =>
    //         {
    //             SelectTarget(target);
    //         });
    //     }
    // }
    //
    // public void SelectTarget(PlayerRef target)
    // {
    //     playerChoicePanel.SetActive(false);
    //
    //     Broadcaster.Instance.RPC_AttackPlayerNotify(localPlayer, target);
    // }
    //
    // public void ShowPlayerSelectPanel()
    // {
    //     playerChoicePanel.SetActive(true);
    // }
    //
    // public void ShowMissedPanel(bool hasMissed, PlayerRef attackPlayerRef, PlayerRef targetPlayerRef)
    // {
    //     useMissedButton.onClick.RemoveAllListeners();
    //     dontUseMissedButton.onClick.RemoveAllListeners();
    //     
    //     waitingPanel.SetActive(false);
    //     missedPanel.SetActive(true);
    //
    //     useMissedButton.enabled = hasMissed;
    //     
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible    = true;
    //     
    //     useMissedButton.onClick.AddListener(() =>
    //     {
    //         missedPanel.SetActive(false);
    //         waitingPanel.SetActive(true);
    //
    //         useMissed = true;
    //         
    //         Broadcaster.Instance.RPC_BroadcastMissedUsage(useMissed, attackPlayerRef, targetPlayerRef);
    //     });
    //     
    //     dontUseMissedButton.onClick.AddListener(() =>
    //     {
    //         missedPanel.SetActive(false);
    //         waitingPanel.SetActive(true);
    //         
    //         useMissed = false;
    //         
    //         Broadcaster.Instance.RPC_MakeCombatEvent(attackPlayerRef, Server.Instance._runner.LocalPlayer, 1);
    //         Broadcaster.Instance.RPC_BroadcastMissedUsage(useMissed, attackPlayerRef, targetPlayerRef);
    //     });
    // }
    //
    // private Action<int> _onCardSelectedCallback;
    //
    // public void ShowCardSelectionPanel(Action<int> onCardSelectedID)
    // {
    //     cardListPanel.SetActive(true);
    //     waitingPanel.SetActive(false);
    //
    //     _onCardSelectedCallback = onCardSelectedID;
    // }
    //
    // public void OnCardSelected(int index)
    // {
    //     cardListPanel.SetActive(false);
    //     
    //     _onCardSelectedCallback?.Invoke(index);
    //     _onCardSelectedCallback = null;
    // }
    //
    // public void ShowWaitingForTargetPanel()
    // {
    //     cardListPanel.SetActive(false);
    //     waitingPanel.SetActive(true);
    // }
    //
    // public void ShowCardTargetPanel()
    // {
    //     cardListPanel.SetActive(true);
    //     waitingPanel.SetActive(false);
    // }
}
