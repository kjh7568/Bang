using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerGameStat playerGameStat;
    [SerializeField] private PlayerBasicStat playerBasicStat;
    
    public PlayerGameStat GameStat => playerGameStat;
    public PlayerBasicStat BasicStat => playerBasicStat;
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveHandCards(int[] cardIDs, RpcInfo info = default)
    {
        if (!Object.HasInputAuthority) return;

        var cards = new CardData[cardIDs.Length];
        
        for (int i = 0; i < cardIDs.Length; i++)
        {
            cards[i] = CardUIManager.Instance.GetCardByID(cardIDs[i]);
        }

        GameStat.InGameStat.HandCards = cards;

        CardUIManager.Instance.SetHandCardImageList();
    }
}
