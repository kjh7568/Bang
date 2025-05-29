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

    public GameObject deadPanel;

    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private TMP_Text resultText;

    public TMP_Text[] resultPlayerNameText;

    public static Dictionary<PlayerRef, string> NicknameCache = new();

    [SerializeField] private MyInfoPanel myInfoPanel;

    [SerializeField] private GameObject saloonPanel;
    [SerializeField] private Button allSaloonButton;
    [SerializeField] private Button mySaloonButton;

    public bool isPanelOn = false;
    public bool isSelectedMissedPanel;
    
    private void Awake()
    {
        Instance = this;

        endTurnButton.onClick.AddListener(() => { Broadcaster.Instance.RPC_RequestEndTurn(); });
        allSaloonButton.onClick.AddListener(() =>
        {
            Broadcaster.Instance.RPC_RequestSaloon(Player.LocalPlayer.playerRef, false);
            OnAndOffSaloonPanel();
        });
        mySaloonButton.onClick.AddListener(() =>
        {
            Broadcaster.Instance.RPC_RequestSaloon(Player.LocalPlayer.playerRef, true);
            OnAndOffSaloonPanel();
        });
    }

    private void Start()
    {
        Broadcaster.Instance.RPC_RequestAllNicknames(Player.LocalPlayer.playerRef);
    }

    public void ResetPanel()
    {
        cardListPanel.SetActive(false);
        waitingPanel.SetActive(false);
        targetPanel.SetActive(false);
        missedPanel.SetActive(false);
    }

    public void OnCardClicked(int index)
    {
        SoundManager.Instance.PlaySound(SoundType.Button);

        PlayerRef playerRef = Player.LocalPlayer.playerRef;
        int cardID = Player.GetPlayer(playerRef).InGameStat.HandCardsId[index];
        var card = CardSystem.Instance.GetCardByIDOrNull(cardID);

        if (Player.LocalPlayer.InGameStat.isBang && card.Name == "Bang") return;

        cardListPanel.SetActive(false);
        
        Broadcaster.Instance.RequestUseCard(Player.LocalPlayer.playerRef, index);
        Player.LocalPlayer.InGameStat.HandCardsId[index] = 0;
        UpdateHandCardUI(Player.LocalPlayer.InGameStat.HandCardsId);

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
            button.GetComponentInChildren<TMP_Text>().text = NicknameCache[playerRef];

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
        useMissedButton.onClick.RemoveAllListeners();
        dontUseMissedButton.onClick.RemoveAllListeners();

        useMissedButton.interactable = hasMissed;

        missedPanel.SetActive(true);

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
                    Broadcaster.Instance.RequestUseCard(Player.LocalPlayer.playerRef, i);
                    Player.LocalPlayer.InGameStat.HandCardsId[i] = 0;
                    UpdateHandCardUI(Player.LocalPlayer.InGameStat.HandCardsId);
                    Broadcaster.Instance.RequestBangAim(attackRef , targetRef);
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
            Broadcaster.Instance.RequestBangAim(attackRef , targetRef);
            Broadcaster.Instance.RPC_NotifyBang(attackRef, targetRef);
        });
    }
    
    public void ShowMissedPanel_Gatling(bool hasMissed, PlayerRef attackRef, PlayerRef targetRef)
    {
        isSelectedMissedPanel = false;
        
        useMissedButton.onClick.RemoveAllListeners();
        dontUseMissedButton.onClick.RemoveAllListeners();

        useMissedButton.interactable = hasMissed;

        missedPanel.SetActive(true);

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
                    isSelectedMissedPanel = true;
                    
                    Broadcaster.Instance.RequestUseCard(Player.LocalPlayer.playerRef, i);
                    Player.LocalPlayer.InGameStat.HandCardsId[i] = 0;
                    UpdateHandCardUI(Player.LocalPlayer.InGameStat.HandCardsId);
                    Broadcaster.Instance.RequestBangAim(attackRef , targetRef);
                    Broadcaster.Instance.RPC_NotifyMissed_Gatling(attackRef, targetRef);
                    
                    return;
                }
            }
        });

        dontUseMissedButton.onClick.AddListener(() =>
        {
            missedPanel.SetActive(false);
            waitingPanel.SetActive(true);

            isSelectedMissedPanel = true;
            
            Player.GetPlayer(targetRef).InGameStat.hp--;
            Broadcaster.Instance.RequestBangAim(attackRef , targetRef);
            Broadcaster.Instance.RPC_NotifyGatling(attackRef, targetRef);
        });
    }
    
    public void ShowResultPanel(string result)
    {
        ResultPanel.SetActive(true);
        resultText.text = result;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
            }
            else
            {
                cardListPanel.SetActive(true);
                isPanelOn = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            myInfoPanel.OpenMyPanel();
        }
    }

    public void SetResultPlayerNames()
    {
        List<Player> players = Player.ConnectedPlayers;
        int outlawIndex = 1;

        for (int i = 0; i < players.Count; i++)
        {
            Player connectedPlayer = players[i];
            if (connectedPlayer == null || connectedPlayer.InGameStat == null ||
                connectedPlayer.InGameStat.MyJob == null)
            {
                
                continue;
            }

            string job = connectedPlayer.InGameStat.MyJob.Name;
            string nickname = connectedPlayer.BasicStat.nickName;

            if (job == "보안관" && resultPlayerNameText[0] != null)
                resultPlayerNameText[0].text = nickname;
            else if (job == "무법자" && outlawIndex <= 2 && resultPlayerNameText[outlawIndex] != null)
                resultPlayerNameText[outlawIndex++].text = nickname;
            else if (job == "배신자" && resultPlayerNameText[3] != null)
                resultPlayerNameText[3].text = nickname;
        }
    }

    public void OnAndOffSaloonPanel()
    {
        saloonPanel.SetActive(!saloonPanel.activeSelf);
    }
}