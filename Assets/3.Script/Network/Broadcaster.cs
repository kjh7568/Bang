using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Broadcaster : NetworkBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateNicknames(string[] nicknames)
    {
        WatingSetting ui = FindObjectOfType<WatingSetting>();
        if (ui != null)
            ui.UpdateNicknameTexts(nicknames);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendMessageToHost(string message, RpcInfo info = default)
    {
        Debug.Log($"[서버] {info.Source} → 닉네임 수신: {message}");
        BasicSpawner.Instance.nicknameBuffer.Add(message);
    }
}