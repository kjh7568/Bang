using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SavePlayerBasicStat : NetworkBehaviour
{
    
    public string Email;
    public string Password;
    public string Nickname;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SendNicknameToServer(string nickname, RpcInfo info = default)
    {
        PlayerRef playerRef = info.Source;

        // 서버에서 등록
        if (BasicSpawner.Instance != null)
        {
            BasicSpawner.Instance.ReceiveNicknameFromClient(playerRef, nickname);
        }
    }
}
