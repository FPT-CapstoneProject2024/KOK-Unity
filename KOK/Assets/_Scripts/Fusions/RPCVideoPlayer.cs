using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using KOK;
using KOK.UISprite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YoutubePlayer.Components;
using static Unity.Collections.Unicode;

public class RPCVideoPlayer : NetworkBehaviour
{

    public static VideoPlayer videoPlayer { get; set; }

    public static bool isVideoPrepared = false;

    private static Button playVideoButton;

    private static TMP_Dropdown songDropDown;

    public static List<PlayerStats> playerNames = new();

    private void Awake()
    {
        videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    private void Start()
    {

        playVideoButton = GameObject.Find("PlayVideoButton").GetComponent<Button>();
        songDropDown = GameObject.Find("SongDropdown").GetComponent<TMP_Dropdown>();
    }

    [Rpc]
    public static void Rpc_Play(NetworkRunner runner, string url)
    {
        videoPlayer.url = url;
        videoPlayer.Stop();
        videoPlayer.Prepare();

        videoPlayer.Play();
        videoPlayer.SetDirectAudioVolume(0, 0.4f);


        Rpc_StartSyncVideo(runner);
    }

    [Rpc]
    public static void Rpc_OnPlayVideoButtonClick(NetworkRunner runner, string url, int songIndex)
    {
        
        
        if (videoPlayer.isPlaying)
        {
            Rpc_Stop(runner);
            Rpc_StopSyncVideo(runner);
        }
        else
        {
            Rpc_Play(runner, url);
        }

        playVideoButton.GetComponent<ButtonSwapSprite>().SwapSprite();
        songDropDown.value = songIndex;

    }


    

    [Rpc]
    public static void Rpc_Stop(NetworkRunner runner)
    {

        videoPlayer.Stop();
        playVideoButton.GetComponent<ButtonSwapSprite>().SwapSprite();
    }



    [Rpc]
    public static void Rpc_TestPlayerList(NetworkRunner runner, int a)
    {
        List<PlayerStats> playerStats = FindObjectsOfType<PlayerStats>().ToList();
        string testString = "";
        foreach (PlayerStats playerStat in playerStats)
        {
            testString += playerStat.PlayerName + " | ";
        }
        Debug.Log(testString);
    }

    [Rpc]
    public static void Rpc_DebugLog(NetworkRunner runner, string content)
    {
        List<PlayerStats> list = FindObjectsOfType<PlayerStats>().ToList();
        List<PlayerRef> playerRefs = runner.ActivePlayers.ToList();
        string test = "";
        foreach (PlayerRef playerRef in playerRefs)
        {
            test += playerRef.PlayerId;
        }
        Debug.Log(content + " | " + runner);
    }

    [Rpc]
    public static void Rpc_StartSyncVideo(NetworkRunner runner)
    {
        PlayerRef host = runner.ActivePlayers.ToArray()[0];
        runner = NetworkRunner.Instances[0];
        foreach (PlayerRef player in runner.ActivePlayers)
        {
            if (runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerRole == 0)
            {
                host = player;
            }
        }
        SyncManager sync = runner.GetPlayerObject(host).GetComponent<SyncManager>();
        if (sync == null)
        {
            sync = runner.GetPlayerObject(host).AddComponent<SyncManager>();
        }
        sync.StartSyncVideo();
    }
    
    [Rpc]
    public static void Rpc_StopSyncVideo(NetworkRunner runner)
    {
        PlayerRef host = runner.ActivePlayers.ToArray()[0];
        runner = NetworkRunner.Instances[0];
        foreach (PlayerRef player in runner.ActivePlayers)
        {
            if (runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerRole == 0)
            {
                host = player;
            }
        }
        SyncManager sync = runner.GetPlayerObject(host).GetComponent<SyncManager>();
        if (sync == null)
        {
            sync = runner.GetPlayerObject(host).AddComponent<SyncManager>();
        }
        sync.StopSyncVideo() ;
    }

    [Rpc] 
    public static void Rpc_TestAddLocalObject(NetworkRunner runner, PlayerStats playerStats)
    {
        List<PlayerRef> playerRefs = runner.ActivePlayers.ToList();
        string test = "";
        foreach (PlayerRef playerRef in playerRefs)
        {
            test += playerRef.PlayerId + " | ";
        }
        //Debug.Log(playerStats.PlayerName + " | " + test + " ======================================");
    }
}


