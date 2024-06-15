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
    }

    private void FixedUpdate()
    {


    }




}
