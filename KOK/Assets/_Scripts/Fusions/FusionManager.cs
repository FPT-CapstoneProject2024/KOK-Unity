using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using KOK;
using KOK.Assets._Scripts;
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
using static Unity.Collections.Unicode;
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
    [SerializeField] GameObject itemPanelCanvas;
    [SerializeField] GameObject popUpCanvas;
    [SerializeField] GameObject videoPlayer;


    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] Button createButton;
    [SerializeField] Button joinButton;
    [SerializeField] Button randomJoinButton;
    [SerializeField] Button playVideoButton;
    [SerializeField] TextMeshProUGUI roomIdTMP;
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (connectOnAwake == true)
        {
            ConnectToRunner("Anonymous");
        }
    }

    private void Start()
    {
        lobbyCanvas.gameObject.SetActive(true);
        clientCanvas.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(false);
        popUpCanvas.gameObject.SetActive(false);
        itemPanelCanvas.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);

        runner = FindAnyObjectByType<NetworkRunner>();
        OnJoinLobby();
        OnRoomNameTMPValueChange();
        createButton.interactable = false;
        roomNameInput.interactable = false;
        randomJoinButton.interactable = false;
    }

    public void OnLoginSuccess()
    {
        lobbyCanvas.gameObject.SetActive(true);
        clientCanvas.gameObject.SetActive(false);
        gameManager.gameObject.SetActive(false);
        popUpCanvas.gameObject.SetActive(false);
        itemPanelCanvas.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);

        //songListDropdown.AddOptions(SongManager.songs.Select(x => x.songName).ToList());

        //OnRoomListDropdownValueChange();
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
        //var roomName = _roomList[roomListDropdown.value].Name.ToString();
        //if (roomName.IsNullOrEmpty())
        //{
        //    roomName = "Nameless Room";
        //}
        //ConnectToRunner(_playerName, roomName);
    }
    
    public void JoinRoom(string roomName)
    {
        playerRole = 1;
        _playerName = nameInput.text;
        if (_playerName.IsNullOrEmpty())
        {
            _playerName = "Anonymous";
        }
        if (roomName.IsNullOrEmpty())
        {
            roomName = "Nameless Room";
        }
        ConnectToRunner(_playerName, roomName);
    }

    public void JoinRoomRandom()
    {
        if (_roomList != null && _roomList.Count > 0)
        {
            int rd = Random.Range(0, _roomList.Count);
            _playerName = nameInput.text;
            ConnectToRunner(_playerName, _roomList[rd].Name);
        }
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

        //joinButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
        //joinButton.interactable = false;


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
        popUpCanvas.gameObject.SetActive(false);
        itemPanelCanvas.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        //OnRoomListDropdownValueChange();
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

    public void UpdateRoomList(List<SessionInfo> sessionList)
    {
        FindAnyObjectByType<RoomListUpdate>().UpdateRoomList(sessionList);
    }
    
   

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //playerObject = runner.Spawn(playerPrefab, new Vector3(spawnPoint.position.x + Random.Range(-spawnOffset, spawnOffset), 0, spawnPoint.position.z + Random.Range(-spawnOffset, spawnOffset)));

        //_playerName = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName);

        playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity);

        if (runner.ActivePlayers.Count() > 1)
        {
            playerRole = 1;
        }
        else
        {
            playerRole = 0;
        }
        runner.SetPlayerObject(runner.LocalPlayer, playerObject);

        roomNameInput.interactable = false;
        randomJoinButton.interactable = false;
        lobbyCanvas.gameObject.SetActive(false);
        clientCanvas.gameObject.SetActive(true);
        popUpCanvas.gameObject.SetActive(true);
        itemPanelCanvas.gameObject.SetActive(true);
        videoPlayer.gameObject.SetActive(true);
        gameManager.gameObject.SetActive(true);

        playerObject.transform.localPosition = spawnPoint.localPosition;
        playerObject.GetComponent<PlayerNetworkBehavior>().PlayerName = _playerName;
        playerObject.GetComponent<PlayerNetworkBehavior>().PlayerColor = _playerColor;
        StartCoroutine(WaitToSetPosition());
    }
    
    IEnumerator WaitToSetPosition()
    {
        yield return new WaitForSeconds(0.5f);
        playerObject.transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        playerObject.transform.position = Vector3.zero;
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
        FindAnyObjectByType<RoomListUpdate>().ClearRoomList();
        createButton.interactable = false;
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
        try
        {
            ParticipantItemHandlerManager.Instance.UpdateParticipantList();
        }
        catch { }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
        string test = runner.ActivePlayers.ToList().OrderBy(x => x.ToString()).ToList()[0].ToString();
        Debug.LogError("OnPlayerLeft | " + test);
        runner.GetPlayerObject(runner.ActivePlayers.ToList().OrderBy(x => x.ToString()).ToList()[0]).GetComponent<PlayerNetworkBehavior>().PlayerRole = 0;   
        ParticipantItemHandlerManager.Instance.UpdateParticipantList();

        ChatManager.Instance.SendMessageAll(playerObject.GetComponent<PlayerNetworkBehavior>().PlayerName + " has left");
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
        //roomListDropdown.ClearOptions();
        //roomListDropdown.AddOptions(_roomList.Select(x => x.Name).ToList());
        //OnRoomListDropdownValueChange();
        UpdateRoomList(sessionList);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
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

    private void AssignHostRoleRandom(NetworkRunner runner)
    {
        int random = Random.Range(0, runner.ActivePlayers.Count() - 1);
        Debug.LogError(runner.ActivePlayers.Count() +"=========================");
        int i = 0;
        foreach (var player in runner.ActivePlayers)
        {
            RPCVideoPlayer.Rpc_DebugLog(runner, runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerName.ToString());
            if (random == i)
            {
                runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole = 0;
            }
            else
            {
                runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole = 1;
            }
            i++;
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
            roomNameInput.interactable = true;
            randomJoinButton.interactable = true;
            FindAnyObjectByType<RoomListUpdate>().UpdateRoomList(_roomList);
        }

    }

    public void OnPlayVideoButtonClick()
    {
        RPCSongManager.Rpc_SetSingerAuto(runner);
        if (RPCVideoPlayer.isPlaying())
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
        string url = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().GetSongURLToPlay();
        RPCVideoPlayer.Rpc_OnPlayVideoButtonClick(runner, url);
        RPCSongManager.Rpc_RemoveSong(runner, 0);
        foreach (var item in FindObjectsByType<SongItemManager>(FindObjectsSortMode.None))
        {
            item.UpdateQueueSongList();
            item.UpdateSongList();
        };

    }
    public void OnJumpToNextVideoButtonClick()
    {
        RPCSongManager.Rpc_SetSingerAuto(runner);
        if (RPCVideoPlayer.isPlaying())
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

        RPCSongManager.Rpc_StopRecording(runner);
    }

    public void AddSongToQueue(string songCode, string singer1Name, string singer2Name)
    {
        RPCSongManager.Rpc_AddSong(runner, songCode, singer1Name, singer2Name);
    }

    public void PrioritizeSongToQueue(string songCode, string singer1Name, string singer2Name)
    {
        RPCSongManager.Rpc_PrioritizeSong(runner, songCode, singer1Name, singer2Name);
    }

    public void RemoveSongFromQueue(int index)
    {
        RPCSongManager.Rpc_RemoveSong(runner, index);
    }


    public void TestDebug()
    {
        RPCVideoPlayer.Rpc_DebugLog(runner, _playerName);
    }
}
