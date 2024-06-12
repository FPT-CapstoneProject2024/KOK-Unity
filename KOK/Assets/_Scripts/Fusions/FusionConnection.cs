using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using static Fusion.NetworkRunnerCallbackArgs;
using Random = UnityEngine.Random;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionConnection Instance;
    public bool connectOnAwake = false;
    public NetworkRunner runner;
    [SerializeField] NetworkObject playerPrefab;

    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas audioCanvas;
    [SerializeField] Canvas videoCanvas;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField roomIdInput;
    [SerializeField] Button joinButton;
    [SerializeField] TextMeshProUGUI roomIdTMP;

    [SerializeField] float spawnOffset = 0f;
    public string _playerName = "Anonymous";
    private string _sessionName = "";
    public Color _playerColor;


    private void Awake()
    {
        _playerColor = UnityEngine.Random.ColorHSV();
        if (Instance == null)
        {
            Instance = this;
        }
        if (connectOnAwake == true)
        {
            ConnectToRunner("Anonymous");
        }
    }

    private void Start()
    {
        lobbyCanvas.gameObject.SetActive(true);
        audioCanvas.gameObject.SetActive(false);
        videoCanvas.gameObject.SetActive(false);
    }

    public void ConnectToServer()
    {
        _playerName = nameInput.text;
        if (_playerName.IsNullOrEmpty())
        {
            _playerName = "Anonymous";
        }
        _sessionName = roomIdInput.text;
        if (_sessionName.IsNullOrEmpty())
        {
            ConnectToRunner(_playerName);
        } else
        {
            ConnectToRunner(_playerName, _sessionName);
        }
         
        joinButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
        joinButton.interactable = false;
        

    }

    public async void ConnectToRunner(string playerName)
    {
        _playerName = playerName;

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            
            PlayerCount = 10
        });
        roomIdTMP.text = "Room id: " + runner.SessionInfo.Name;
    }
    
    public async void ConnectToRunner(string playerName, string roomId)
    {
        _playerName = playerName;

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomId,
            PlayerCount = 10,
            
        });
        roomIdTMP.text = "Room id: " + runner.SessionInfo.Name;
    }

    

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
        NetworkObject playerObject = runner.Spawn(playerPrefab, new Vector3(Random.Range(-spawnOffset, spawnOffset), 0 , Random.Range(-spawnOffset, spawnOffset)));
        playerObject.name = "Player: " + _playerName;

        runner.SetPlayerObject(runner.LocalPlayer, playerObject);

        lobbyCanvas.gameObject.SetActive(false);
        audioCanvas.gameObject.SetActive(true);
        videoCanvas.gameObject.SetActive(true);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("OnInput");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("OnInputMissing");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("OnObjectEnterAOI");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("OnObjectExitAOI");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log("OnReliableDataProgress");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        Debug.Log("OnReliableDataReceived");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadStart");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    public static void TestRPC()
    {
        
        RPCVideoPlayerDemo.Rpc_TestPlayerList(FindAnyObjectByType<NetworkRunner>(), 1);
    }

    public static void Rpc_PrepareVideo()
    {
        RPCVideoPlayerDemo.Rpc_Prepare(FindAnyObjectByType<NetworkRunner>(), 1);
    }
}
