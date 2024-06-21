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

    [Networked][SerializeField] public int PlayerRole { get; set; }

    [SerializeField] public VideoPlayer videoPlayer;

    [Networked] public double videoTime { get; set; }

    [Networked] PlayState playState { get; set; }

    

    private void Start()
    {
        if (this.HasStateAuthority)
        {
            PlayerName = FusionConnection.Instance._playerName;
            PlayerColor = FusionConnection.Instance._playerColor;
            PlayerRole = FusionConnection.Instance.playerRole;
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

    public int CompareTo(PlayerStats obj)
    {
        return PlayerName.Compare(obj.PlayerName);
    }

    public int Compare(PlayerStats x, PlayerStats y)
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
}
