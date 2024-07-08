using Fusion;
using KOK;
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

public class PlayerNetworkBehavior : NetworkBehaviour, IComparable<PlayerNetworkBehavior>, IComparer<PlayerNetworkBehavior> 
{
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

    [Networked, Capacity(100)][SerializeField] public NetworkArray<NetworkString<_32>> QueueSongCodeList { get; } = MakeInitializer(new NetworkString<_32>[] { "S005", "S009", "S004", });



    private void Start()
    {
        if (this.HasStateAuthority)
        {
            PlayerName = FusionManager.Instance._playerName;
            PlayerColor = FusionManager.Instance._playerColor;
            PlayerRole = FusionManager.Instance.playerRole;
            CharacterCode = "";
            AvatarCode = "DemoAvatar";
        }
        playerNameLabel.text = PlayerName.ToString();
        playerNameLabel.color = PlayerColor;
        playerRenderer.color = PlayerColor;

        videoPlayer = FindAnyObjectByType<VideoPlayer>();

        RPCVideoPlayer.Rpc_TestAddLocalObject(FindAnyObjectByType<NetworkRunner>(), this);

        StartCoroutine(UpdateTime());
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
        videoPlayer.time = videoTime;
    }

    public void Rpc_PrepareVideo()
    {
        videoPlayer.Prepare();
    }

    public void Rpc_PlayVideo()
    {
        videoPlayer.Play();
    }

    public void Rpc_StopVideo()
    {
        videoPlayer.Stop();
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
        DemoSong song = SongManager.GetSongBySongCode(songCode.ToString());

        NetworkString<_32>[] tmp = QueueSongCodeList.Where((source, index) => index != 0).ToArray();
        QueueSongCodeList.CopyFrom(tmp, 0, tmp.Count());
        return song.songURL;
    }
    public void AddSongToQueue(string songCode)
    {

    }

    public void RemoveSongFromQueue(int index)
    {

    }

    public void ClearSongQueue()
    {

    }

    public void MoveSongInQueueToIndex(int index)
    {

    }

    public void StopSong()
    {

    }

    //public string GetNextSongURLToPlay()
    //{

    //}

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
}
