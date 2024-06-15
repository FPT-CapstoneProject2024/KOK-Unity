using Fusion;
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

public class RPCVideoPlayerDemo : NetworkBehaviour
{

    public static VideoPlayer videoPlayer { get; set; }

    public static bool isVideoPrepared = false;

    private static Button playVideoButton;

    private void Awake()
    {
        //invidiousVideoPlayer = FindAnyObjectByType<InvidiousVideoPlayer>();
        videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    private void Start()
    {

        playVideoButton = GameObject.Find("PlayVideoButton").GetComponent<Button>();
    }

    [Rpc]
    public static async void Rpc_Play(NetworkRunner runner, string url)
    {
        videoPlayer.url = url;
        videoPlayer.Stop();
        videoPlayer.Prepare();
        videoPlayer.Play();
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
        playVideoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Video";
    }

    [Rpc]
    public static async void Rpc_OnPlayVideoButtonClick(NetworkRunner runner, string url)
    {
        if (videoPlayer.isPlaying)
        {
             Rpc_Stop(runner);
            
            
        }
        else
        {
            Rpc_Play(runner, url);
        }
    }


    [Rpc]
    public static async void Rpc_Stop(NetworkRunner runner)
    {
        
        videoPlayer.Stop();
        playVideoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Video";
    }

    

    [Rpc] 
    public static void Rpc_TestPlayerList(NetworkRunner runner, int a)
    {
        List<PlayerStats> playerStats = FindObjectsOfType<PlayerStats>().ToList();
        string testString = "";
        foreach(PlayerStats playerStat in playerStats)
        {
            testString += playerStat.PlayerName + " | ";
        }
        Debug.Log(testString);
    }
}
