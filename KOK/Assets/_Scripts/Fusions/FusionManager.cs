using Fusion;
using Fusion.Sockets;
using KOK;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using static Fusion.NetworkRunnerCallbackArgs;
using Random = UnityEngine.Random;

public class FusionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionManager Instance;
    public bool connectOnAwake = false;
    public NetworkRunner runner;
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] GameObject runnerPrefab;

    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas clientCanvas;
    [SerializeField] GameObject gameManager;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] Button createButton;
    [SerializeField] Button joinButton;
    [SerializeField] Button randomJoinButton;
    [SerializeField] Button playVideoButton;
    [SerializeField] TextMeshProUGUI roomIdTMP;
    [SerializeField] TMP_Dropdown roomListDropdown;
    [SerializeField] TMP_Dropdown songListDropdown;
    [SerializeField] Transform spawnPoint;

    [SerializeField] float spawnOffset = 0f;
    public string _playerName = "Anonymous";
    private string _sessionName = "";
    public Color _playerColor;
    private List<SessionInfo> _roomList = new List<SessionInfo>();
    private PlayerInputHandler _localPlayerInputHandler;
    public int playerRole = 0;

    NetworkObject playerObject;

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
        lobbyCanvas.gameObject.SetActive(false);
        clientCanvas.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(false);

        runner = FindAnyObjectByType<NetworkRunner>();
        OnJoinLobby();
    }

    public void OnLoginSuccess()
    {
        lobbyCanvas.gameObject.SetActive(true);
        clientCanvas.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(false);

        roomListDropdown.ClearOptions();
        songListDropdown.ClearOptions();
        //songListDropdown.AddOptions(SongManager.songs.Select(x => x.songName).ToList());

        OnRoomNameTMPValueChange();
        OnRoomListDropdownValueChange();
    }

    public void CreateRoom()
    {
        playerRole = 0;
        _playerName = nameInput.text;
        if (_playerName.IsNullOrEmpty())
        {
            _playerName = "Anonymous";
        }
        var roomName = roomNameInput.text;
        if (roomName.IsNullOrEmpty())
        {
            roomName = "Nameless Room";
        }
        ConnectToRunner(_playerName, roomName);
    }

    public void JoinRoom()
    {
        playerRole = 1;
        _playerName = nameInput.text;
        if (_playerName.IsNullOrEmpty())
        {
            _playerName = "Anonymous";
        }
        var roomName = _roomList[roomListDropdown.value].Name.ToString();
        if (roomName.IsNullOrEmpty())
        {
            roomName = "Nameless Room";
        }
        ConnectToRunner(_playerName, roomName);
    }

    public void ConnectToServer()
    {
        _playerName = nameInput.text;
        if (_playerName.IsNullOrEmpty())
        {
            _playerName = "Anonymous";
        }
        _sessionName = roomNameInput.text;
        if (_sessionName.IsNullOrEmpty())
        {
            ConnectToRunner(_playerName);
        }
        else
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

        runner.ProvideInput = true;
        roomIdTMP.text = "Room: " + runner.SessionInfo.Name;
    }

    public async void ConnectToRunner(string playerName, string roomName)
    {
        _playerName = playerName;

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = 10,

        });

        runner.ProvideInput = true;
        roomIdTMP.text = "Room: " + runner.SessionInfo.Name;
    }

    public async void DisconnectFromRoom()
    {
        runner.Despawn(runner.GetPlayerObject(runner.LocalPlayer));
        await runner.Shutdown(true, ShutdownReason.Ok);
        var newRunner = Instantiate(runnerPrefab);
        newRunner.name = "NetworkRunner";
        runner = newRunner.GetComponent<NetworkRunner>();
        lobbyCanvas.gameObject.SetActive(true);
        clientCanvas.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(false);
        OnRoomListDropdownValueChange();
        _sessionName = "";
        OnJoinLobby();

    }

    public void OnRoomNameTMPValueChange()
    {
        if (roomNameInput.text.IsNullOrEmpty())
        {
            createButton.interactable = false;
        }
        else
        {
            createButton.interactable = true;
        }
    }

    public void OnRoomListDropdownValueChange()
    {
        if (_roomList.Count == 0)
        {
            joinButton.interactable = false;
            randomJoinButton.interactable = false;
        }
        else
        {
            joinButton.interactable = true;
            randomJoinButton.interactable = true;
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //playerObject = runner.Spawn(playerPrefab, new Vector3(spawnPoint.position.x + Random.Range(-spawnOffset, spawnOffset), 0, spawnPoint.position.z + Random.Range(-spawnOffset, spawnOffset)));
        playerObject = runner.Spawn(playerPrefab, Vector3.zero);

        //playerObject.transform.parent = spawnPoint;
        playerObject.name = "Player: " + _playerName;
        playerObject.GetComponentInChildren<TextMeshPro>().text = _playerName;
        playerObject.GetComponent<SpriteRenderer>().color = _playerColor;
        playerObject.GetComponentInChildren<TextMeshPro>().color = _playerColor;
        runner.SetPlayerObject(runner.LocalPlayer, playerObject);


        lobbyCanvas.gameObject.SetActive(false);
        clientCanvas.gameObject.SetActive(true);
        gameManager.gameObject.SetActive(true);

        playerObject.transform.localPosition = spawnPoint.localPosition;
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
        ParticipantItemHandlerManager.Instance.UpdateParticipantList();

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
        _roomList = sessionList;
        roomListDropdown.ClearOptions();
        roomListDropdown.AddOptions(_roomList.Select(x => x.Name).ToList());
        OnRoomListDropdownValueChange();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
        _sessionName = runner.SessionInfo.Name;

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    private void OnApplicationPause(bool pause)
    {
        if (runner.IsShutdown)
        {
            if (!_sessionName.IsNullOrEmpty())
            {
                ConnectToRunner(_playerName, _sessionName);
            }
        }
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("JoinLobby stated");
        string lobbyId = "DefaultLobbyId";
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, lobbyId);
        if (!result.Ok)
        {
            Debug.LogError("Unable to join lobby " + lobbyId);
        }
        else
        {
            Debug.Log("JoinLobby Ok");
        }
    }

    public void OnPlayVideoButtonClick()
    {
        if (RPCVideoPlayer.videoPlayer.isPlaying)
        {
            StopVideo();
        }
        else
        {
            PlayVideo();
        }
    }
    private void PlayVideo()
    {
        //string url = SongManager.songs[songListDropdown.value].songURL;
        string url = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().GetSongURLToPlay();
        RPCVideoPlayer.Rpc_OnPlayVideoButtonClick(runner, url);

    }
    public void OnJumpToNextVideoButtonClick()
    {
        if (RPCVideoPlayer.videoPlayer.isPlaying)
        {
            StopVideo();
            PlayVideo();
        }
        else
        {
            PlayVideo();
        }
    }

    private void StopVideo()
    {
        RPCVideoPlayer.Rpc_Stop(runner);
    }

    public void TestDebug()
    {
        RPCVideoPlayer.Rpc_DebugLog(runner, _playerName);
    }
}