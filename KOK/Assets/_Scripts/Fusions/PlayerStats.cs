using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YoutubePlayer.Components;

public class PlayerStats : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [SerializeField] TextMeshPro playerNameLabel;
    [Networked] public Color PlayerColor { get; set; }
    [SerializeField] SpriteRenderer playerRenderer;

    //InvidiousVideoPlayer InvidiousVideoPlayer { get; set; }
    [SerializeField] VideoPlayer VideoPlayer { get; set; }

    private void Start()
    {
        //InvidiousVideoPlayer = FindAnyObjectByType<InvidiousVideoPlayer>();
        //InvidiousVideoPlayer.InvidiousInstance = FindObjectOfType<InvidiousVideoPlayer>().InvidiousInstance;

        VideoPlayer = FindAnyObjectByType<VideoPlayer>();
        VideoPlayer.targetCamera = Camera.main;
        //this.InvidiousVideoPlayer.VideoPlayer = VideoPlayer;
        if (this.HasStateAuthority)
        {
            PlayerName = FusionConnection.Instance._playerName;
            PlayerColor = FusionConnection.Instance._playerColor;
        }
        //yield return new WaitUntil(() => this.isActiveAndEnabled);
        playerNameLabel.text = PlayerName.ToString();
        playerRenderer.color = PlayerColor;
        //Debug.Log("Video Preparing ======================================================================");
        //yield return new WaitUntil(() => VideoPlayer.isPrepared);
        //Debug.Log("Complete video prepare ==============================================================");
    }

    private void FixedUpdate()
    {


    }

    
    public async Task PrepareVideo()
    {
        Debug.Log(name + ": Loading video...");
        //await InvidiousVideoPlayer.PrepareVideoAsync();
        Debug.Log(name + ": Video ready");
        
    }

    public void EnablePlayVideoButton()
    {
        Button playButton = GameObject.Find("PlayVideoButton").GetComponent<Button>();
        playButton.interactable = true;
    }


}
