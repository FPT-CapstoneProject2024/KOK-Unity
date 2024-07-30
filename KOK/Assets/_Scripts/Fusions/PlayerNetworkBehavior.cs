using Fusion;
using KOK;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Audio;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
using WebSocketSharp;
using YoutubePlayer.Components;
using static Unity.Collections.Unicode;

public class PlayerNetworkBehavior : NetworkBehaviour, IComparable<PlayerNetworkBehavior>, IComparer<PlayerNetworkBehavior>
{
    [Networked] public NetworkString<_64> AccountId { get; set; }
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [SerializeField] TextMeshPro playerNameLabel;
    [Networked] public Color PlayerColor { get; set; }
    [SerializeField] SpriteRenderer playerRenderer;

    [Networked][SerializeField] public int PlayerRole { get; set; }

    [SerializeField] public VideoPlayer videoPlayer;

    [Networked] public double videoTime { get; set; }

    [Networked] PlayState playState { get; set; }

    [Networked][SerializeField] public string CharacterCode { get; set; }

    [Networked][SerializeField] public string AvatarCode { get; set; }

    [Networked] public int queueSongCount { get; set; }
    [Networked] public int singer1Count { get; set; }
    [Networked] public int singer2Count { get; set; }
    [Networked] public bool isAutoNextSong { get; set; }
    [Networked] public int nextSongCountDown { get; set; }

    [Networked] public bool isSinger { get; set; }

    [Networked, Capacity(50)][SerializeField] public NetworkArray<NetworkString<_32>> QueueSongCodeList { get; }
    [Networked, Capacity(50)][SerializeField] public NetworkArray<NetworkString<_32>> QueueSinger1List { get; }
    [Networked, Capacity(50)][SerializeField] public NetworkArray<NetworkString<_32>> QueueSinger2List { get; }

    [Networked] public NetworkString<_128> audioUrl { get; set; }

    public List<SongDetail> SongList { get; private set; }

    public List<SongDetail> QueueSongList { get; private set; }
    public List<SongDetail> PurchasedSongList { get; private set; }
    public List<FavoriteSong> FavoriteSongList { get; private set; }

    public string RoomLogString { get; private set; }

    [SerializeField] private VoiceRecorder voiceRecorder { get; set; }

    private void Start()
    {

        if (this.HasStateAuthority)
        {
            //Debug.LogError(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName));
            LoadSongList();
            ClearSongQueue();
            NetworkRunner runner = NetworkRunner.Instances[0];
            //PlayerName = FusionManager.Instance._playerName;
            //PlayerColor = FusionManager.Instance._playerColor;

            AccountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            PlayerName = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName);
            if (runner.ActivePlayers.Count() > 1)
            {
                PlayerRole = 1;
                StartCoroutine(SyncSongQueueWithHost());
                StartCoroutine(SyncRoomLogWithHost());
            }
            else
            {
                PlayerRole = 0;
                //string logFileName = "RoomLog_" + PlayerName.ToString() + "_" + DateTime.Now + ".txt";
                //logFileName = logFileName.Replace(" ", "");
                //logFileName = logFileName.Replace(":", "");
                //logFileName = logFileName.Replace("/", "");
                //RoomLogManager.Instance.CreateRoomLog(logFileName, Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)));
            }

            CharacterCode = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharacterItemId);
            AvatarCode = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharacterItemId);


            videoPlayer = FindAnyObjectByType<VideoPlayer>();

            //RPCVideoPlayer.Rpc_TestAddLocalObject(FindAnyObjectByType<NetworkRunner>(), this);

            SetSinger();

            StartCoroutine(UpdateTime());
            StartCoroutine(UpdateSearchSongUI());
            //StartCoroutine(NotiJoinRoom());
            RoomLogString = "";
            voiceRecorder = FindAnyObjectByType<VoiceRecorder>();
        }
        this.name = "Player: " + PlayerName; playerNameLabel.text = PlayerName.ToString();
        playerNameLabel.color = PlayerColor;
        playerRenderer.color = PlayerColor;

    }

    IEnumerator NotiJoinRoom()
    {
        yield return new WaitForSeconds(2f);
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.SendMessageAll(PlayerName + " has joined");
        }
        else
        {
            StartCoroutine(NotiJoinRoom());
        }
    }

    private void FixedUpdate()
    {


    }

    public int CompareTo(PlayerNetworkBehavior obj)
    {
        return PlayerName.Compare(obj.PlayerName);
    }

    public int Compare(PlayerNetworkBehavior x, PlayerNetworkBehavior y)
    {
        return x.PlayerName.Compare(y.PlayerName);
    }

    private void LoadSongList()
    {
        SongList = new();
        FindAnyObjectByType<ApiHelper>().gameObject
                    .GetComponent<SongController>()
                    .GetSongsFilterPagingCoroutine(new SongFilter(),
                                                    SongOrderFilter.SongName,
                                                    new PagingRequest(),
                                                    (list) => { SongList = list; StartCoroutine(UpdateSearchSongUI()); },
                                                    (ex) => Debug.LogError(ex));
        //Load favorite and purchased song list here
        FavoriteSongList = new();
        //FindAnyObjectByType<ApiHelper>().gameObject
        //                .GetComponent<FavoriteSongController>()
        //                .GetMemberFavoriteSongCoroutine(new FavoriteSongFilter() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)) },
        //                                                FavoriteSongOrderFilter.SongId,
        //                                                new PagingRequest(),
        //                                                (list) => { FavoriteSongList = list.Results; StartCoroutine(UpdateSearchSongUI()); },
        //                                                (ex) => Debug.LogError(ex));

        PurchasedSongList = new();


    }


    public SongDetail GetSongBySongCode(string songCode)
    {
        return SongList.FirstOrDefault(x => x.SongCode.Equals(songCode.ToString()));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_SyncVideoTime()
    {
        videoTime = videoPlayer.time;
    }
    IEnumerator UpdateTime()
    {
        yield return new WaitForSeconds(0.5f);
        Rpc_SyncVideoTime();
        StartCoroutine(UpdateTime());
    }

    public void Rpc_SetVideoPlayerSyncTime()
    {
        if (videoPlayer != null)
        {
            videoPlayer.time = videoTime;
        }
    }

    public void Rpc_PrepareVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
        }
    }

    public void Rpc_PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    public void Rpc_StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    public string GetSongURLToPlay()
    {
        //Get song url, play for all member and remove from queue
        if (NetworkArray_IsEmpty(QueueSongCodeList))
        {
            Debug.Log("Queue is empty!");
            return "";
        }
        NetworkString<_32> songCode = QueueSongCodeList[0];
        SongDetail song = GetSongBySongCode(songCode.ToString());

        return song.SongUrl;
    }

    public void AddSongToQueue(string songCode, string singer1Name, string singer2Name)
    {

        CountQueue();
        if (PlayerRole == 0)
        {
            QueueSongCodeList.Set(queueSongCount, songCode);
            QueueSinger1List.Set(singer1Count, singer1Name);
            QueueSinger2List.Set(singer2Count, singer2Name);
            StartCoroutine(UpdateQueueSongUI());
        }
        else
        {
            StartCoroutine(SyncSongQueueWithHost());
        }
        CountQueue();
    }

    public void PrioritizeSongToQueue(string songCode, string singer1Name, string singer2Name)
    {
        var runner = FindAnyObjectByType<NetworkRunner>();
        if (PlayerRole == 0)
        {
            CountQueue();
            for (int i = queueSongCount; i > 0; i--)
            {
                QueueSongCodeList.Set(i, QueueSongCodeList[i - 1]);
                QueueSinger1List.Set(i, QueueSinger1List[i - 1]);
                QueueSinger2List.Set(i, QueueSinger2List[i - 1]);
            }
            QueueSongCodeList.Set(0, songCode);
            QueueSinger1List.Set(0, singer1Name);
            QueueSinger2List.Set(0, singer2Name);
            CountQueue();
            StartCoroutine(UpdateQueueSongUI());
        }
        else
        {
            StartCoroutine(SyncSongQueueWithHost());
            CountQueue();
        }
    }
    public void RemoveSongFromQueue(int index)
    {
        if (PlayerRole == 0)
        {
            NetworkString<_32> songCode = QueueSongCodeList[0];
            DemoSong song = DemoSongManager.GetSongBySongCode(songCode.ToString());

            NetworkString<_32>[] tmp = QueueSongCodeList.Where((source, i) => i != index).ToArray();
            QueueSongCodeList.Clear();
            QueueSongCodeList.CopyFrom(tmp, 0, tmp.Count());

            tmp = QueueSinger1List.Where((source, i) => i != index).ToArray();
            QueueSinger1List.Clear();
            QueueSinger1List.CopyFrom(tmp, 0, tmp.Count());

            tmp = QueueSinger2List.Where((source, i) => i != index).ToArray();
            QueueSinger2List.Clear();
            QueueSinger2List.CopyFrom(tmp, 0, tmp.Count());
            StartCoroutine(UpdateQueueSongUI());
        }
        else
        {
            StartCoroutine(SyncSongQueueWithHost());
        }
        CountQueue();
    }

    public void ClearSongQueue()
    {
        if (PlayerRole == 0)
        {
            QueueSongCodeList.Clear();
            QueueSinger1List.Clear();
            QueueSinger2List.Clear();
            StartCoroutine(UpdateQueueSongUI());
        }
        else
        {
            StartCoroutine(SyncSongQueueWithHost());
        }
    }

    public void MoveSongInQueueToIndex(int moveIndex, int targetIndex)
    {

    }

    public void CountDownToNextSong()
    {

    }

    private void CountQueue()
    {
        queueSongCount = 0;
        singer1Count = 0;
        singer2Count = 0;
        foreach (var song in QueueSongCodeList)
        {
            if (song.ToString().Trim() == "")
            {
                break;
            }
            else
            {
                queueSongCount++;
            }
        }
        foreach (var singer in QueueSinger1List)
        {
            if (singer.ToString().Trim() == "")
            {
                break;
            }
            else
            {
                singer1Count++;
            }
        }
        foreach (var singer in QueueSinger2List)
        {
            if (singer.ToString().Trim() == "")
            {
                break;
            }
            else
            {
                singer2Count++;
            }
        }
        //Debug.LogError(QueueSongCodeList.ToCommaSeparatedString()
        //             + QueueSinger1List.ToCommaSeparatedString()
        //             + QueueSinger2List.ToCommaSeparatedString());
    }

    private bool NetworkArray_IsEmpty(NetworkArray<NetworkString<_32>> networkArray)
    {
        bool isEmpty = true;
        foreach (var item in networkArray)
        {
            if (!item.ToString().IsNullOrEmpty())
            {
                return false;
            }
        }
        return isEmpty;
    }

    private void NetworkArray_RemoveAt(ref NetworkArray<NetworkString<_32>> networkArray, int index)
    {
        NetworkString<_32>[] tmp = QueueSongCodeList.Where((source, index) => index != 0).ToArray();
        networkArray.CopyFrom(tmp, 0, tmp.Count());
    }

    IEnumerator SyncSongQueueWithHost()
    {
        yield return new WaitForSeconds(1f);
        try
        {
            var runner = NetworkRunner.Instances[0];

            foreach (var player in runner.ActivePlayers)
            {
                if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole == 0)
                {
                    QueueSongCodeList.Clear();
                    for (int i = 0; i < QueueSongCodeList.Length; i++)
                    {
                        QueueSongCodeList.Set(i, runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().QueueSongCodeList[i]);
                    }
                    QueueSinger1List.Clear();
                    for (int i = 0; i < QueueSinger1List.Length; i++)
                    {
                        QueueSinger1List.Set(i, runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().QueueSinger1List[i]);
                    }
                    QueueSinger2List.Clear();
                    for (int i = 0; i < QueueSinger2List.Length; i++)
                    {
                        QueueSinger2List.Set(i, runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().QueueSinger2List[i]);
                    }
                    break;
                }
            }
            GameObject.Find("QueueToggle").GetComponent<SongItemManager>().UpdateQueueSongList();
        }
        catch (Exception) { };


    }

    IEnumerator UpdateQueueSongUI()
    {
        yield return new WaitForSeconds(1f);
        FindAnyObjectByType<SongItemManager>().UpdateQueueSongList();
    }

    public void RefreshSearchSongUI()
    {
        StartCoroutine(UpdateSearchSongUI());
    }


    IEnumerator UpdateSearchSongUI()
    {
        yield return new WaitForSeconds(1f);
        FindAnyObjectByType<SongItemManager>().UpdateSongList();
    }
    public void SetSinger()
    {
        if (QueueSinger1List[0].ToString() == PlayerName || QueueSinger2List[0].ToString() == PlayerName)
        {
            isSinger = true;
        }
        else { isSinger = false; }
        //Debug.LogError(QueueSinger1List[0].ToString() + " | " + PlayerName + " | " + isSinger);
        FindAnyObjectByType<RoomClientController>().CheckSinger(isSinger);

    }

    public void UpdateRoomLog(string newLog)
    {
        if (PlayerRole == 0)
        {
            RoomLogString += newLog;
            RoomLogManager.Instance.AppendLogToFile(newLog);
        }
    }

    IEnumerator SyncRoomLogWithHost()
    {
        yield return new WaitForSeconds(1f);
        try
        {
            var runner = NetworkRunner.Instances[0];

            foreach (var player in runner.ActivePlayers)
            {
                if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole == 0)
                {
                    RoomLogString = runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().RoomLogString;
                    break;
                }
            }
            GameObject.Find("QueueToggle").GetComponent<SongItemManager>().UpdateQueueSongList();
        }
        catch (Exception) { };
    }

    public void StartRecording()
    {
        if (isSinger)
        {
            var audioFile = QueueSongCodeList[0] + "_" + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName) + "_" + DateTime.Now.ToString();
            audioFile = audioFile.Replace(" ", "");
            audioFile = audioFile.Replace(":", "");
            audioFile = audioFile.Replace("/", "");

            if (voiceRecorder == null) voiceRecorder = FindAnyObjectByType<VoiceRecorder>();
            voiceRecorder.StartRecording(audioFile);

            audioUrl = audioFile;
        }

        if (PlayerRole == 0)
        {
            StartCoroutine(CreateRecording());
        }
    }

    IEnumerator CreateRecording()
    {
        yield return new WaitForSeconds(5f);
        if (PlayerRole == 0)
        {
            List<string> singerAudioUrls = new();
            List<string> singerAccountIds = new();
            var runner = NetworkRunner.Instances[0];
            foreach (var player in runner.ActivePlayers)
            {
                if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().isSinger)
                {
                    singerAudioUrls.Add(runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().audioUrl.ToString());
                    singerAccountIds.Add(runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().AccountId.ToString());
                }
            }

            RecordingManager.Instance.CreateRecording(
                    "Record_" + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName),
                    UnityEngine.Random.Range(50, 100),
                    "ebe4174c-5069-4767-a5c3-a962563d813f",
                    PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                    PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                    RoomLogManager.Instance.roomId.ToString(),
                    singerAudioUrls,
                    singerAccountIds
                    );
        }
    }

    public void StopRecording()
    {
        if (voiceRecorder == null) voiceRecorder = FindAnyObjectByType<VoiceRecorder>();
        voiceRecorder.StopRecording();
    }


}


