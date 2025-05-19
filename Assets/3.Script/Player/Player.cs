using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerGameStat playerGameStat;
    [SerializeField] private PlayerBasicStat playerBasicStat;
    
    public PlayerGameStat GameStat => playerGameStat;
    public PlayerBasicStat BasicStat => playerBasicStat;
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReceiveHandCards(int[] cards)
    {
        Debug.Log($"[{BasicStat.nickName}] 핸드카드 수신");

        for (int i = 0; i < cards.Length; i++)
        {
            GameStat.InGameStat.HandCardsId[i] = cards[i];
        }

        if (Object.HasInputAuthority)
        {
            // 내 클라이언트에서만 UI 갱신
            CardUIManager.Instance.SetHandCardImageList();
        }
        
        //CardUIManager.Instance.SetHandCardImageList();
    }
}
