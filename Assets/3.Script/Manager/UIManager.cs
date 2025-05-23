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

    
    private void Awake()
    {
        Instance = this;
        playerPanel.SetActive(false); 
    }
    
    [SerializeField] Button targetButtonPrefab; 
    [SerializeField] Transform buttonParent; 
    
    public PlayerRef localPlayer;

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
        playerPanel.SetActive(false);

        Broadcaster.Instance.RPC_AttackPlayerNotify(localPlayer, target);
    }

    public void ShowPlayerSelectPanel(Action<string> onTargetSelectedCallback)
    {
        playerPanel.SetActive(true);
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
}
