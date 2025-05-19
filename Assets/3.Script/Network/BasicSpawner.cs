using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static BasicSpawner Instance { get; private set; }
    
    [SerializeField] private GameObject[] playerPrefabs;
    public NetworkRunner _runner;
    
    public Dictionary<PlayerRef, NetworkObject> spawnedPlayers;
    // public Dictionary<int, NetworkObject> spawnedDummyPlayers;
    
    private string sessionNumber;
    private Dictionary<PlayerRef, bool> readyStates = new();
    
    private void Awake()
    {
        spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
        // spawnedDummyPlayers = new Dictionary<int, NetworkObject>();
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void StartGame(GameMode mode)
    {
        string randSessionName = Random.Range(0, 10000).ToString();
        
        //멀티플레이 세션을 만들어야 함 -> 포톤의 주요 컴포넌트들을 셋팅해야함 -> runner
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true; // 인풋에 대한 정보를 받을것인가?를 true로 함
        
        var scene = SceneRef.FromIndex(2);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid) //해당 씬이 빌드에 포함 되어있나? -> 빌드의 인덱스를 가지고 있는가?
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            //args 초기화
            GameMode = mode,
            SessionName = randSessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }
    
    public async void StartGame(GameMode mode, string sessionName)
    {
        //멀티플레이 세션을 만들어야 함 -> 포톤의 주요 컴포넌트들을 셋팅해야함 -> runner
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true; // 인풋에 대한 정보를 받을것인가?를 true로 함
        
        var scene = SceneRef.FromIndex(2);
        var sceneInfo = new NetworkSceneInfo();
        
        if (scene.IsValid) //해당 씬이 빌드에 포함 되어있나? -> 빌드의 인덱스를 가지고 있는가?
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            //args 초기화
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }
    
    public string GetSessionNumber() => _runner.SessionInfo.Name;
    
    private void BroadcastNicknamesToAll()
    {
        List<string> nicknames = new();

        foreach (var kvp in spawnedPlayers)
        {
            var playerComponent = kvp.Value.GetComponent<Player>();
            if (playerComponent != null)
            {
                string nick = playerComponent.BasicStat.nickName;
                // Debug.Log($"[Server] 닉네임 추가: {nick}");
                nicknames.Add(nick);
            }
            else
            {
                Debug.LogWarning("[Server] Player 컴포넌트가 없음!");
            }
        }

        // Debug.Log($"[Server] 총 닉네임 수: {nicknames.Count}");
        RPC_UpdateNicknames(nicknames.ToArray());
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateNicknames(string[] nicknames)
    {
        var ui = FindObjectOfType<WatingSetting>();
        if (ui != null)
        {
            ui.UpdateNicknameTexts(new List<string>(nicknames));
        }
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (_runner.IsServer)
        {
            int prefabIdx = _runner.ActivePlayers.Count() - 1;
            var spawnTransform = playerPrefabs[prefabIdx].transform;
            
            var networkPlayer = runner.Spawn(playerPrefabs[prefabIdx], spawnTransform.position, spawnTransform.rotation, player);
            spawnedPlayers.Add(player, networkPlayer);
            
            // spawnedDummyPlayers.Add(0, networkPlayer);
            
            DontDestroyOnLoad(networkPlayer);
            
            // if (_runner.ActivePlayers.Count() == 1)
            // {
            //     for (int i = 1; i < 4; i++)
            //     {
            //         int dummyIdx = i;
            //         var dummyTransform = playerPrefabs[dummyIdx].transform;
            //         var dummyPlayer = runner.Spawn(playerPrefabs[dummyIdx], dummyTransform.position, dummyTransform.rotation);
            //
            //         spawnedDummyPlayers.Add(i, dummyPlayer);
            //
            //         DontDestroyOnLoad(dummyPlayer);
            //         // Debug.Log($"더미 플레이어 {i} 생성 완료");
            //     }
            // }
            
            if (_runner.ActivePlayers.Count() == 4)
            {
                WatingSetting ui = FindObjectOfType<WatingSetting>();
                if (ui != null)
                    ui.ShowStartButton();
            }
            
            // if (spawnedDummyPlayers.Count() == 4)
            // {
            //     WatingSetting ui = FindObjectOfType<WatingSetting>();
            //     if (ui != null)
            //         ui.ShowStartButton();
            // }
            
            BroadcastNicknamesToAll();
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.ContainsKey(player))
        {
            runner.Despawn(spawnedPlayers[player]);
            spawnedPlayers.Remove(player);
            BroadcastNicknamesToAll();
        }
    }
    
    #region interface methods
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    #endregion
}
