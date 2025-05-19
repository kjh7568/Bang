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
    public Dictionary<PlayerRef, NetworkObject> playerNickNames;
    
    private string sessionNumber;
    private Dictionary<PlayerRef, bool> readyStates = new();
    
    private void Awake()
    {
        spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
        
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
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (_runner.IsServer)
        {
            int prefabIdx = _runner.ActivePlayers.Count() - 1;
            var spawnTransform = playerPrefabs[prefabIdx].transform;
            
            var networkPlayer = runner.Spawn(playerPrefabs[prefabIdx], spawnTransform.position, spawnTransform.rotation, player);
            spawnedPlayers.Add(player, networkPlayer);
            
            DontDestroyOnLoad(networkPlayer);
            
            if (spawnedPlayers.Count() == 4)
            {
                WatingSetting ui = FindObjectOfType<WatingSetting>();
                if (ui != null)
                    ui.ShowStartButton();
            }
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.ContainsKey(player))
        {
            runner.Despawn(spawnedPlayers[player]);
            spawnedPlayers.Remove(player);
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
