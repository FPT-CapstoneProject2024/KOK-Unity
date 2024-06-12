using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using YoutubePlayer.Components;

public class RPCVideoPlayerDemo : NetworkBehaviour
{
    //public static InvidiousVideoPlayer invidiousVideoPlayer { get; set; }

    public static VideoPlayer videoPlayer { get; set; }

    public static bool isVideoPrepared = false;

    private void Awake()
    {
        //invidiousVideoPlayer = FindAnyObjectByType<InvidiousVideoPlayer>();
        videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    [Rpc]
    public static void Rpc_DebugTestRPC(NetworkRunner runner, int a)
    {
        Debug.Log("Test RPC ================================");
    }

    [Rpc]
    public static async void Rpc_Prepare(NetworkRunner runner, int a)
    {
        ////List<NetworkR>
        //invidiousVideoPlayer = FindAnyObjectByType<InvidiousVideoPlayer>();

        //Debug.Log("Loading video...");
        //await invidiousVideoPlayer.PrepareVideoAsync();
        ////await hết client ở đây
        //Debug.Log("Video ready");
        List<PlayerStats> playerStats = FindObjectsOfType<PlayerStats>().ToList();
        //foreach (PlayerStats playerStat in playerStats)
        //{
        //    await playerStat.PrepareVideo();
        //}

        try
        {
            var tasks = new List<Task>();

            foreach (var playerStat in playerStats)
            {
                tasks.Add(playerStat.PrepareVideo());
            }

            await Task.WhenAll(tasks);

            foreach (PlayerStats playerStat in playerStats)
            {
                Debug.Log(playerStat.name + ": Enable");
                playerStat.EnablePlayVideoButton();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        
    }


    [Rpc]
    public static async void Rpc_Play(NetworkRunner runner, int a)
    {
        //List<PlayerStats> playerStats = FindObjectsOfType<PlayerStats>().ToList();
        //try
        //{
        //    var tasks = new List<Task>();

        //    foreach (var playerStat in playerStats)
        //    {
        //        tasks.Add(videoPlayer.prepareCompleted);

        //    }

        //    await Task.WhenAll(tasks);

        //    foreach (PlayerStats playerStat in playerStats)
        //    {
        //        Debug.Log(playerStat.name + ": Enable");
        //        playerStat.EnablePlayVideoButton();
        //    }

        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.ToString());
        //}
        videoPlayer.Prepare();
        videoPlayer.Play();
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
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
