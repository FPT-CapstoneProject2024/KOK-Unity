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
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
        

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
            Rpc_StartSyncVideo(runner);
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
        Debug.Log(content);
    }

    [Rpc]
    public static void Rpc_StartSyncVideo(NetworkRunner runner)
    {
        SyncManager sync = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<SyncManager>() ?? runner.GetPlayerObject(runner.LocalPlayer).AddComponent<SyncManager>();
        sync.StartSyncVideo() ;
    }
    
    [Rpc]
    public static void Rpc_StopSyncVideo(NetworkRunner runner)
    {
        SyncManager sync = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<SyncManager>() ?? runner.GetPlayerObject(runner.LocalPlayer).AddComponent<SyncManager>();
        sync.StopSyncVideo() ;
    }
}


