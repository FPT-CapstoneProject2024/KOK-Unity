using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
using YoutubePlayer.Components;

public class PlayerStats : NetworkBehaviour, IComparable<PlayerStats>, IComparer<PlayerStats>
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [SerializeField] TextMeshPro playerNameLabel;
    [Networked] public Color PlayerColor { get; set; }
    [SerializeField] SpriteRenderer playerRenderer;

    [SerializeField] int roleInRoom;

    [SerializeField] public VideoPlayer videoPlayer;

    public double videoTime = 0;

    [Networked] PlayState playState { get; set; }

    

    private void Start()
    {
        if (this.HasStateAuthority)
        {
            PlayerName = FusionConnection.Instance._playerName;
            PlayerColor = FusionConnection.Instance._playerColor;
        }
        playerNameLabel.text = PlayerName.ToString();
        playerNameLabel.color = PlayerColor;
        playerRenderer.color = PlayerColor;

        videoPlayer = FindAnyObjectByType<VideoPlayer>();
    }

    private void FixedUpdate()
    {


    }

    public int CompareTo(PlayerStats obj)
    {
        return PlayerName.Compare(obj.PlayerName);
    }

    public int Compare(PlayerStats x, PlayerStats y)
    {
        return x.PlayerName.Compare(y.PlayerName);
    }

    IEnumerator UpdateTime()
    {
        yield return new WaitForSeconds(3);
        videoPlayer = FindAnyObjectByType<VideoPlayer>();
        videoTime = videoPlayer.time;
        StartCoroutine(UpdateTime());
    }
}
