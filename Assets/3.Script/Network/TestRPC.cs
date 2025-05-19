using Fusion;
using UnityEngine;

public class TestRPC : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendDataToHost(string message, int value)
    {
        Debug.Log($"[Host] 클라이언트로부터 받은 메시지: {message}, 값: {value}");
    }
}
